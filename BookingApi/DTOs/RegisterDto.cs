using System.ComponentModel.DataAnnotations;

namespace BookingApi.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Password { get; set; } = string.Empty;
}