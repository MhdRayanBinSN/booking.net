using BookingApi.Application.DTOs;

namespace BookingApi.Application.Interfaces;

public interface IBookingService
{
    Task<List<BookingResponseDto>> GetAll();

    Task<BookingResponseDto?> GetById(int id, int userId);

    Task<BookingResponseDto?> Create(CreateBookingDto dto, int userId);

    Task<BookingResponseDto?> Update(
     int id,
     UpdateBookingDto dto,
     int userId,
     string role);

    Task<bool> Delete(int id, int userId, string role);
}