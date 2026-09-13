using BookingApi.Infrastructure.Persistence;
using BookingApi.Interfaces;
using BookingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BookingApi.DTOs;
namespace BookingApi.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;
    private readonly ILogger<BookingService> _logger;
    public BookingService(
        BookingDbContext context,
        ILogger<BookingService> logger

        )
    {
        _context = context;
        _logger = logger;
    }


    // GET ALL
    public async Task<List<BookingResponseDto>> GetAll()
    {
        return await _context.Bookings
        .AsNoTracking()
        .Select(b => new BookingResponseDto
        {
            Id = b.Id,
            CustomerName = b.Customer.Name,
            Room = b.Room,
            BookingDate = b.BookingDate,
            IsConfirmed = b.IsConfirmed
        })
        .ToListAsync();
    }

    // GET BY ID
    public async Task<BookingResponseDto?> GetById(int id, int userId)
    {
        return await _context.Bookings
        .Where(b =>
            b.Id == id &&
            b.Customer.UserId == userId)
        .Select(b => new BookingResponseDto
        {
            Id = b.Id,
            CustomerName = b.Customer.Name,
            Room = b.Room,
            BookingDate = b.BookingDate,
            IsConfirmed = b.IsConfirmed
        })
        .FirstOrDefaultAsync();
    }

    // CREATE
    public async Task<BookingResponseDto?> Create(CreateBookingDto dto,
     int userId)
    {


        var customer = await _context.Customers
        .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            _logger.LogWarning(
                "Customer profile was not found for user {UserId}",
                userId);

            return null;
        }

        var booking = new Booking
        {
            CustomerId = customer.Id,
            Room = dto.Room,
            BookingDate = dto.BookingDate,
            IsConfirmed = dto.IsConfirmed
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Booking {BookingId} created successfully for customer {CustomerId}",
            booking.Id,
            customer.Id);

        var createdBooking = await _context.Bookings
            .Include(b => b.Customer)
            .FirstAsync(b => b.Id == booking.Id);

        return new BookingResponseDto
        {
            Id = createdBooking.Id,
            CustomerName = createdBooking.Customer.Name,
            Room = createdBooking.Room,
            BookingDate = createdBooking.BookingDate,
            IsConfirmed = createdBooking.IsConfirmed
        };
    }

    // UPDATE
    public async Task<BookingResponseDto?> Update(
    int id,
    UpdateBookingDto dto,
    int userId,
    string role)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return null;

        if (role != "Admin" &&
            booking.Customer.UserId != userId)
        {
            return null;
        }

        booking.Room = dto.Room;
        booking.BookingDate = dto.BookingDate;
        booking.IsConfirmed = dto.IsConfirmed;

        await _context.SaveChangesAsync();

        return new BookingResponseDto
        {
            Id = booking.Id,
            CustomerName = booking.Customer.Name,
            Room = booking.Room,
            BookingDate = booking.BookingDate,
            IsConfirmed = booking.IsConfirmed
        };
    }

    // DELETE
    public async Task<bool> Delete(int id, int userId, string role)
    {

        var booking = await _context.Bookings
        .Include(b => b.Customer)
        .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
            return false;

        //admin - delete acces
        if (role != "Admin" && booking.Customer.UserId != userId)
        {
            return false;
        }

        _context.Bookings.Remove(booking);

        await _context.SaveChangesAsync();

        return true;
    }
}