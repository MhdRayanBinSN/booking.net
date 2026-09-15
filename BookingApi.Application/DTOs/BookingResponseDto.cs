namespace BookingApi.Application.DTOs;

public class BookingResponseDto
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string Room { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public bool IsConfirmed { get; set; }
}