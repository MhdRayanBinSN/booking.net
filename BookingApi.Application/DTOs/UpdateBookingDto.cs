namespace BookingApi.Application.DTOs;

public class UpdateBookingDto
{
    public int CustomerId { get; set; }

    public string Room { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public bool IsConfirmed { get; set; }
}