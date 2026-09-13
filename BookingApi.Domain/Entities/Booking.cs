namespace BookingApi.Domain.Entities;

public class Booking
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public string Room { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public bool IsConfirmed { get; set; }
}