using HelpDeskPro.Api.Dtos;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskPro.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserRepository users, ITicketRepository tickets) : ControllerBase
{
    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await users.GetAllAsync()).Select(MapToResponse));

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(MapToResponse(user));
    }

    // GET /api/users/{id}/tickets
    [HttpGet("{id}/tickets")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTickets(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null) return NotFound($"User {id} nicht gefunden.");

        var userTickets = await tickets.GetByUserIdAsync(id);
        return Ok(userTickets.Select(t => new
        {
            t.Id,
            t.Title,
            t.Status,
            t.Priority,
            t.CreatedAt
        }));
    }

    private static UserResponse MapToResponse(User u) =>
        new(u.Id, u.Name, u.Email, u.Role);
}
