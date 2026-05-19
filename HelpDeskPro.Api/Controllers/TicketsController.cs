using HelpDeskPro.Api.Dtos;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;
using HelpDeskPro.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HelpDeskPro.Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController(
    ITicketRepository tickets,
    ICommentRepository comments,
    IEmailService emailService,
    StatusTransitionService statusService,
    TicketFilterService filterService) : ControllerBase
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
        var full = await tickets.GetByIdAsync(created.Id);
        await emailService.SendTicketCreatedAsync(full!);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(full!));
    }

    // PATCH /api/tickets/{id}/status
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound($"Ticket {id} nicht gefunden.");

        var (ok, error) = statusService.Validate(ticket.Status, request.NewStatus);
        if (!ok) return BadRequest(error);

        var oldStatus = ticket.Status;
        ticket.Status = request.NewStatus;
        var updated = await tickets.UpdateAsync(ticket);
        var full = await tickets.GetByIdAsync(updated.Id);
        await emailService.SendStatusChangedAsync(full!, oldStatus);
        return Ok(MapToResponse(full!));
    }

    // PATCH /api/tickets/{id}/assign
    [HttpPatch("{id}/assign")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignTicketRequest request)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound($"Ticket {id} nicht gefunden.");

        ticket.AssignedToUserId = request.AssignedToUserId == 0 ? null : request.AssignedToUserId;
        var updated = await tickets.UpdateAsync(ticket);
        var full = await tickets.GetByIdAsync(updated.Id);
        return Ok(MapToResponse(full!));
    }

    // POST /api/tickets/{id}/comments
    [HttpPost("{id}/comments")]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest request)
    {
        var ticket = await tickets.GetByIdAsync(id);
        if (ticket is null) return NotFound($"Ticket {id} nicht gefunden.");

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Kommentar darf nicht leer sein.");

        var comment = new Comment { TicketId = id, Text = request.Text, AuthorId = request.AuthorId };
        var created = await comments.AddAsync(comment);
        var full = await tickets.GetByIdAsync(id);
        await emailService.SendCommentAddedAsync(full!, created);

        return CreatedAtAction(nameof(GetById), new { id },
            new CommentResponse(created.Id, created.Text, created.CreatedAt,
                created.AuthorId, created.Author?.Name ?? "–"));
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

    // GET /api/tickets?status=Open&priority=High&search=drucker
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] int? assignedToUserId,
        [FromQuery] int? createdByUserId,
        [FromQuery] string? search)
    {
        var all = await tickets.GetAllAsync();
        var filter = new TicketFilter
        {
            Status = status is not null && Enum.TryParse<Core.Enums.TicketStatus>(status, out var s) ? s : null,
            Priority = priority is not null && Enum.TryParse<Core.Enums.Priority>(priority, out var p) ? p : null,
            AssignedToUserId = assignedToUserId,
            CreatedByUserId = createdByUserId,
            SearchText = search
        };
        return Ok(filterService.Apply(all, filter).Select(MapToResponse));
    }

    private static TicketResponse MapToResponse(Ticket t) => new(
        t.Id, t.Title, t.Description, t.Status, t.Priority,
        t.CreatedAt, t.UpdatedAt,
        t.CreatedByUserId, t.CreatedBy?.Name ?? "–",
        t.AssignedToUserId, t.AssignedTo?.Name,
        t.Comments.Select(c => new CommentResponse(c.Id, c.Text, c.CreatedAt, c.AuthorId, c.Author?.Name ?? "–"))
    );
}
