using BookingApi.DTOs;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace BookingApi.Services;

public class AuthService
{
    private readonly BookingDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(BookingDbContext context,
    IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> Register(RegisterDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Username == dto.Username ||
                u.Email == dto.Email
            );
        if (existingUser != null)
        {
            return null;
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Role = "Customer"
        };
        user.PasswordHash = _passwordHasher.HashPassword(
            user, dto.Password
        );
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var customer = new Customer
        {
            UserId = user.Id,
            Name = user.Username,
            Email = user.Email
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();



        return user;
    }
    public async Task<User?> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        return user;
    }
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
    public string HashRefreshToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash);
    }

}