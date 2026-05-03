// This file contains code for ReceptionController.
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
    
    [HttpGet("rooms")]
    public async Task<IActionResult> GetAllRooms(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllRoomsQuery(), ct);
        return Ok(ApiResult.Success(result));
    }

    
    [HttpGet("reservations/{id:guid}")]
    public async Task<IActionResult> GetReservationDetails(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReservationDetailsQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(ApiResult.Success(result));
    }

    
    [HttpGet("reservations")]
    public async Task<IActionResult> GetActiveReservations(CancellationToken ct)
    {
        var result = await mediator.Send(new GetActiveReservationsQuery(), ct);
        return Ok(ApiResult.Success(result));
    }

    
    [HttpGet("reservations/search")]
    public async Task<IActionResult> FindReservation(
        [FromQuery] string? guestName, [FromQuery] string? qrCode, CancellationToken ct)
    {
        var result = await mediator.Send(new FindReservationQuery(guestName, qrCode), ct);
        return Ok(ApiResult.Success(result));
    }

    
    [HttpPost("walk-in")]
    public async Task<IActionResult> CreateWalkIn(
        [FromBody] CreateWalkInReservationCommand command, CancellationToken ct)
    {
        var reservationId = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(reservationId));
    }

    
    
    
    
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(
        [FromBody] CheckInCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(result));
    }

    
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(
        [FromBody] CheckOutCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(result));
    }
}
