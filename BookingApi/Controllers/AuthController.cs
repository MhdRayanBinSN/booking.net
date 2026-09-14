using BookingApi.DTOs;
using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AuthController(
        AuthService authService,
        JwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var user = await _authService.Register(dto);

        if (user == null)
            return Conflict("Username or email already exists.");

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var user = await _authService.Login(dto);

        if (user == null)
            return Unauthorized("Invalid username or password.");

        var accessToken = _jwtService.GenerateToken(user);

        var refreshToken =
            await _authService.CreateRefreshToken(user);

        return Ok(new
        {
            token = accessToken,
            refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest("Refresh token is required.");

        var refreshToken =
            await _authService.GetValidRefreshToken(
                dto.RefreshToken);

        if (refreshToken == null)
            return Unauthorized("Invalid refresh token.");

        var newAccessToken =
            _jwtService.GenerateToken(refreshToken.User);

        return Ok(new
        {
            token = newAccessToken
        });
    }
}