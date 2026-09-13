namespace BookingApi.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public User? User { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public List<Booking> Bookings { get; set; } = new();
}