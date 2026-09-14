using BookingApi.Domain.Entities;

namespace BookingApi.Application.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);
    Task<Customer?> GetByUserIdAsync(int userId);
}