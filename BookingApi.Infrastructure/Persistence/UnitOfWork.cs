using BookingApi.Application.Interfaces;

namespace BookingApi.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookingDbContext _context;

    public UnitOfWork(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}