using BookingApi.Application.Interfaces;
using BookingApi.DTOs;
using BookingApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace BookingApi.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
private readonly IRefreshTokenRepository _refreshTokenRepository;
    public AuthService(
    IUserRepository userRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher<User> passwordHasher)
{
    _userRepository = userRepository;
    _customerRepository = customerRepository;
    _unitOfWork = unitOfWork;
    _refreshTokenRepository = refreshTokenRepository;
    _passwordHasher = passwordHasher;
}
    public async Task<User?> Register(RegisterDto dto)
    {
        // 1. Check whether username/email already exists
        var existingUser =
            await _userRepository.GetByUsernameOrEmailAsync(
                dto.Username,
                dto.Email);

        if (existingUser != null)
        {
            return null;
        }

        // 2. Create User
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Role = "Customer"
        };

        // 3. Business/security logic: hash password
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password);

        // 4. Add User through repository
        await _userRepository.AddAsync(user);

        // 5. Save so User gets an Id
        await _unitOfWork.SaveChangesAsync();

        // 6. Create Customer profile
        var customer = new Customer
        {
            UserId = user.Id,
            Name = user.Username,
            Email = user.Email
        };

        // 7. Add Customer through repository
        await _customerRepository.AddAsync(customer);

        // 8. Commit
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<User?> Login(LoginDto dto)
    {
        // 1. Get user through repository
        var user =
            await _userRepository.GetByUsernameAsync(
                dto.Username);

        if (user == null)
            return null;

        // 2. Password verification stays in Service
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

    public async Task<string> CreateRefreshToken(User user)
{
    var refreshToken = GenerateRefreshToken();

    var refreshTokenHash = HashRefreshToken(refreshToken);

    var entity = new RefreshToken
    {
        UserId = user.Id,
        TokenHash = refreshTokenHash,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
    };

    await _refreshTokenRepository.AddAsync(entity);

    await _unitOfWork.SaveChangesAsync();

    return refreshToken;
}
public async Task<RefreshToken?> GetValidRefreshToken(
    string refreshToken)
{
    var tokenHash = HashRefreshToken(refreshToken);

    var token =
        await _refreshTokenRepository.GetByTokenHashAsync(
            tokenHash);

    if (token == null)
        return null;

    if (token.IsRevoked)
        return null;

    if (token.ExpiresAt <= DateTime.UtcNow)
        return null;

    return token;
}
}