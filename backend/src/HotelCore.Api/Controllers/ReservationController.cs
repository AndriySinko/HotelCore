// This file contains code for ReservationController.
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Reception.Commands.CreateReservation;
using HotelCore.Application.Reception.Queries.GetAvailableRooms;
using HotelCore.Domain.Enums;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ReservationController(IMediator mediator) : ControllerBase
{
    [HttpGet("rooms/available")]
    public async Task<IActionResult> GetAvailableRooms(
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] RoomType? type,
        CancellationToken ct)
    {
        var rooms = await mediator.Send(new GetAvailableRoomsQuery(checkIn, checkOut, type), ct);
        return Ok(ApiResult.Success(rooms));
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation(
        [FromBody] CreateReservationCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Ok(ApiResult.Success(result));
    }
}
