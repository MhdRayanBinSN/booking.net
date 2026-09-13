using BookingApi.Domain.Entities;

namespace BookingApi.Application.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();

    Task<Booking?> GetByIdAsync(int id);

    Task AddAsync(Booking booking);

    Task UpdateAsync(Booking booking);

    Task DeleteAsync(Booking booking);
}