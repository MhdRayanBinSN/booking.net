using BookingApi.Domain.Entities;

namespace BookingApi.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameOrEmailAsync(
        string username,
        string email);

    Task<User?> GetByUsernameAsync(string username);

    Task AddAsync(User user);
}