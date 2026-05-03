// all reception desk operations — only Receptionist and Administrator can access these endpoints
// covers room lookup, reservation management, check-in and check-out
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Reception.Commands.CheckIn;
using HotelCore.Application.Reception.Commands.CheckOut;
using HotelCore.Application.Reception.Commands.CreateWalkIn;
using HotelCore.Application.Reception.Queries.FindReservation;
using HotelCore.Application.Reception.Queries.GetActiveReservations;
using HotelCore.Application.Reception.Queries.GetAllRooms;
using HotelCore.Application.Reception.Queries.GetReservationDetails;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Receptionist,Administrator")]
public class ReceptionController(IMediator mediator) : ControllerBase
{
    // returns all rooms with their current status — used to display the room map at reception
    [HttpGet("rooms")]
    public async Task<IActionResult> GetAllRooms(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllRoomsQuery(), ct);
        return Ok(ApiResult.Success(result));
    }

    // full details for a single reservation — room, guest, payments
    [HttpGet("reservations/{id:guid}")]
    public async Task<IActionResult> GetReservationDetails(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReservationDetailsQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(ApiResult.Success(result));
    }

    // returns all reservations currently in Reserved or CheckedIn status, sorted by check-in date
    [HttpGet("reservations")]
    public async Task<IActionResult> GetActiveReservations(CancellationToken ct)
    {
        var result = await mediator.Send(new GetActiveReservationsQuery(), ct);
        return Ok(ApiResult.Success(result));
    }

    // search by guest name or QR code — used at the front desk when a guest arrives without the booking reference
    [HttpGet("reservations/search")]
    public async Task<IActionResult> FindReservation(
        [FromQuery] string? guestName, [FromQuery] string? qrCode, CancellationToken ct)
    {
        var result = await mediator.Send(new FindReservationQuery(guestName, qrCode), ct);
        return Ok(ApiResult.Success(result));
    }

    // creates a reservation on the spot for a guest without a prior booking
    // walk-in reservations skip the online booking flow and go straight to the system
    [HttpPost("walk-in")]
    public async Task<IActionResult> CreateWalkIn(
        [FromBody] CreateWalkInReservationCommand command, CancellationToken ct)
    {
        var reservationId = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(reservationId));
    }

    // processes a guest arrival — verifies identity, assigns the room, creates the payment record
    // may return an alternative room if the original reserved room is not yet ready
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(
        [FromBody] CheckInCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(result));
    }

    // marks the guest as checked out, sets the room to UnderCleaning, and returns the total charged
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(
        [FromBody] CheckOutCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(result));
    }
}
