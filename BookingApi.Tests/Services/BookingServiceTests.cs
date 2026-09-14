using BookingApi.Application.Interfaces;
using BookingApi.Domain.Entities;
using BookingApi.DTOs;
using BookingApi.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingApi.Tests.Services;

public class BookingServiceTests
{
    [Fact]
    public async Task GetById_ReturnsBooking_WhenBookingBelongsToUser()
    {
        // Arrange
        var bookingRepository = new Mock<IBookingRepository>();
        var customerRepository = new Mock<ICustomerRepository>();
        var logger = new Mock<ILogger<BookingService>>();

        var booking = new Booking
        {
            Id = 1,
            Room = "Room 101",
            BookingDate = DateTime.UtcNow,
            IsConfirmed = true,
            Customer = new Customer
            {
                Id = 10,
                UserId = 5,
                Name = "Rayan",
                Email = "rayan@example.com"
            }
        };

        bookingRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(booking);

        var service = new BookingService(
            bookingRepository.Object,
            customerRepository.Object,
            logger.Object);

        // Act
        var result = await service.GetById(1, 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Rayan", result.CustomerName);
        Assert.Equal("Room 101", result.Room);
    }
}