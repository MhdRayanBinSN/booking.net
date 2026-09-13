using BookingApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using BookingApi.Interfaces;
using BookingApi.Services;
using BookingApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    //consstructor
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<BookingResponseDto>>> GetAll()
    {
        var bookings = await _bookingService.GetAll();
        return Ok(bookings);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetById(int id)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var booking = await _bookingService.GetById(id, userId);

        if (booking == null)
        {
            return NotFound();
        }

        return Ok(booking);
    }


    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create(CreateBookingDto dto)
    {
        var userIdClaim = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        var createdBooking = await _bookingService.Create(dto, userId);
        if (createdBooking == null)
        {
            return NotFound("Customer not found.");
        }
        return Ok(createdBooking);
    }

    [Authorize]
[HttpPut("{id}")]
public async Task<ActionResult<BookingResponseDto>> UpdateBooking(
    int id,
    UpdateBookingDto dto)
{
    var userIdClaim = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (!int.TryParse(userIdClaim, out var userId))
        return Unauthorized();

    var role = User.FindFirstValue(
        ClaimTypes.Role);

    if (role == null)
        return Unauthorized();

    var result = await _bookingService.Update(
        id,
        dto,
        userId,
        role);

    if (result == null)
        return Forbid();

    return Ok(result);
}


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {

         var userIdClaim = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (!int.TryParse(userIdClaim, out var userId))
        return Unauthorized();

    var role = User.FindFirstValue(
        ClaimTypes.Role);

    if (role == null)
        return Unauthorized();

    var deleted = await _bookingService.Delete(
        id,
        userId,
        role);

    if (!deleted)
        return Forbid();

    return NoContent();
    }

};