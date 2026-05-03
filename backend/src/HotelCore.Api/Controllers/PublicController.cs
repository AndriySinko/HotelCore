// This file contains code for PublicController.
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Reception.Queries.SearchReservationsPublic;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class PublicController(IMediator mediator) : ControllerBase
{
    
    [HttpGet("reservations/search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(ApiResult.Failure("Search query is required."));

        var result = await mediator.Send(new SearchReservationsPublicQuery(q), ct);
        return Ok(ApiResult.Success(result));
    }
}
