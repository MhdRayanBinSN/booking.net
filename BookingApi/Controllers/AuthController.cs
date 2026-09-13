using BookingApi.DTOs;
using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;
using BookingApi.Infrastructure.Persistence;
using BookingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;
    private readonly BookingDbContext _context;
    public AuthController(
     AuthService authService,
     JwtService jwtService,
     BookingDbContext context)
    {
        _authService = authService;
        _jwtService = jwtService;
        _context = context;
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

        var refreshToken = _authService.GenerateRefreshToken();

        var refreshTokenHash =
            _authService.HashRefreshToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

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

    var tokenHash = _authService.HashRefreshToken(
        dto.RefreshToken);

    var refreshToken = await _context.RefreshTokens
        .Include(r => r.User)
        .FirstOrDefaultAsync(r =>
            r.TokenHash == tokenHash);

    if (refreshToken == null)
        return Unauthorized("Invalid refresh token.");

    if (refreshToken.IsRevoked)
        return Unauthorized("Refresh token has been revoked.");

    if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        return Unauthorized("Refresh token has expired.");

    var newAccessToken =
        _jwtService.GenerateToken(refreshToken.User);

    return Ok(new
    {
        token = newAccessToken
    });
}

}