using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly BookingDbContext _context;

    public CustomerRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public async Task<Customer?> GetByUserIdAsync(int userId)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }
}