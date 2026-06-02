using HelpDeskPro.Api.Dtos;
using HelpDeskPro.Core.Interfaces;
using HelpDeskPro.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskPro.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserRepository users, JwtService jwt) : ControllerBase
{
    /// <summary>
    /// POST /api/auth/login
    /// Authenticates a user with email and password, returns JWT token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validate request
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email und Passwort sind erforderlich.");
        }

        // Find user by email
        var user = await users.GetByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Ungültige E-Mail oder Passwort.");
        }

        // Verify password
        if (!PasswordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Ungültige E-Mail oder Passwort.");
        }

        // Generate JWT token
        var token = jwt.GenerateToken(user);

        var response = new LoginResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            token
        );

        return Ok(response);
    }
}
