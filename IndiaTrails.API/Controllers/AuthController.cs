using IndiaTrails.API.Models.Domain;
using IndiaTrails.API.Models.DTOs.Request;
using IndiaTrails.API.Models.DTOs.Response;
using IndiaTrails.API.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace IndiaTrails.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if user already exists
        if (await _authRepository.UserExistsAsync(request.Email))
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email
        };

        var registeredUser = await _authRepository.RegisterAsync(user, request.Password);

        if (registeredUser == null)
        {
            return BadRequest(new { message = "Registration failed" });
        }

        var token = _authRepository.GenerateJwtToken(registeredUser);

        var response = new AuthResponseDto
        {
            Token = token,
            Username = registeredUser.Username,
            Email = registeredUser.Email
        };

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _authRepository.LoginAsync(request.Email, request.Password);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var token = _authRepository.GenerateJwtToken(user);

        var response = new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(response);
    }
}
