using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BookingDbContext _context;

    public UserRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameOrEmailAsync(
        string username,
        string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Username == username ||
                u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}