using System.ComponentModel.DataAnnotations;
namespace BookingApi.DTOs;

public class CreateBookingDto
{

    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Room { get; set; } = string.Empty;


    public DateTime BookingDate { get; set; }

    public bool IsConfirmed { get; set; }
}