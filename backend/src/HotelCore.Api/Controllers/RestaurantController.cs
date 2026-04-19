using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Common.Usecases.Restaurant.Auth.Commands;

namespace HotelCore.Api.Controllers;

[ApiController]
[Route("api/restaurant")]
public class RestaurantController(IMediator mediator) : ControllerBase
{
    [HttpPost("auth/qr")]
    [AllowAnonymous]
    public async Task<IActionResult> QrLogin(
        [FromBody] QrLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Succeeded)
            return Unauthorized(ApiResult.Failure(result.Error ?? "Authentication failed."));

        return Ok(ApiResult.Success(result));
    }
}
