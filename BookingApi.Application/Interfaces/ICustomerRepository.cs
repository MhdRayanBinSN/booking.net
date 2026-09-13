using BookingApi.Domain.Entities;

namespace BookingApi.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByUserIdAsync(int userId);
}