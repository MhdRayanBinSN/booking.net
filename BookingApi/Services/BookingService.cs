using BookingApi.DTOs;

using BookingApi.Interfaces;
using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;

namespace BookingApi.Services;

public class BookingService : IBookingService
{


    private readonly IBookingRepository _bookingRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<BookingService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public BookingService(
    IBookingRepository bookingRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    ILogger<BookingService> logger)
{
    _bookingRepository = bookingRepository;
    _customerRepository = customerRepository;
    _unitOfWork = unitOfWork;
    _logger = logger;
}


    // GET ALL
    public async Task<List<BookingResponseDto>> GetAll()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        return bookings.Select(b => new BookingResponseDto
        {
            Id = b.Id,
            CustomerName = b.Customer.Name,
            Room = b.Room,
            BookingDate = b.BookingDate,
            IsConfirmed = b.IsConfirmed
        }).ToList();
    }

    // GET BY ID
    public async Task<BookingResponseDto?> GetById(int id, int userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
            return null;

        if (booking.Customer.UserId != userId)
            return null;

        return new BookingResponseDto
        {
            Id = booking.Id,
            CustomerName = booking.Customer.Name,
            Room = booking.Room,
            BookingDate = booking.BookingDate,
            IsConfirmed = booking.IsConfirmed
        };
    }

    // CREATE
    public async Task<BookingResponseDto?> Create(
    CreateBookingDto dto,
    int userId)
{
    // 1. Get customer through repository
    var customer = await _customerRepository.GetByUserIdAsync(userId);

    if (customer == null)
    {
        _logger.LogWarning(
            "Customer profile was not found for user {UserId}",
            userId);

        return null;
    }

    // 2. Business logic: create booking
    var booking = new Booking
    {
        CustomerId = customer.Id,
        Room = dto.Room,
        BookingDate = dto.BookingDate,
        IsConfirmed = dto.IsConfirmed
    };

    // 3. Tell repository to add it
    await _bookingRepository.AddAsync(booking);

    // 4. Commit the transaction
    await _unitOfWork.SaveChangesAsync();

    _logger.LogInformation(
        "Booking {BookingId} created successfully for customer {CustomerId}",
        booking.Id,
        customer.Id);

    // 5. Get the created booking with Customer
    var createdBooking =
        await _bookingRepository.GetByIdAsync(booking.Id);

    if (createdBooking == null)
        return null;

    // 6. Convert entity → DTO
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
        // Get booking through repository
    var booking = await _bookingRepository.GetByIdAsync(id);

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

        await _unitOfWork.SaveChangesAsync();

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
    public async Task<bool> Delete(
    int id,
    int userId,
    string role)
{
    // 1. Get booking through repository
    var booking = await _bookingRepository.GetByIdAsync(id);

    if (booking == null)
        return false;

    // 2. Business/authorization logic
    if (role != "Admin" &&
        booking.Customer.UserId != userId)
    {
        return false;
    }

    // 3. Tell repository to delete
    await _bookingRepository.DeleteAsync(booking);

    // 4. Commit the change
    await _unitOfWork.SaveChangesAsync();

    return true;
}
}