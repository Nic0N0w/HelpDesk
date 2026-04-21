using HelpDeskPro.Api.Dtos;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskPro.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController(ITicketRepository tickets, IEmailService emailService) : ControllerBase
{
    // POST /api/tickets
    [HttpPost]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Titel darf nicht leer sein.");

        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            CreatedByUserId = request.CreatedByUserId
        };

        var created = await tickets.CreateAsync(ticket);
        await emailService.SendTicketCreatedAsync(created);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
    }

    // PATCH /api/tickets/{id}/status
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound($"Ticket {id} nicht gefunden.");

        var oldStatus = ticket.Status;
        ticket.Status = request.NewStatus;

        var updated = await tickets.UpdateAsync(ticket);
        await emailService.SendStatusChangedAsync(updated, oldStatus);

        return Ok(MapToResponse(updated));
    }

    // GET /api/tickets/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await tickets.GetByIdAsync(id);
        return ticket is null ? NotFound() : Ok(MapToResponse(ticket));
    }

    // GET /api/tickets
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await tickets.GetAllAsync()).Select(MapToResponse));

    private static TicketResponse MapToResponse(Ticket t) => new(
        t.Id, t.Title, t.Description, t.Status, t.Priority,
        t.CreatedAt, t.UpdatedAt,
        t.CreatedByUserId, t.CreatedBy?.Name ?? "–",
        t.AssignedToUserId, t.AssignedTo?.Name,
        t.Comments.Select(c => new CommentResponse(c.Id, c.Text, c.CreatedAt, c.AuthorId, c.Author?.Name ?? "–"))
    );
}
