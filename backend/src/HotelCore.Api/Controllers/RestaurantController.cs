using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Common.Usecases.Restaurant.Auth.Commands;
using HotelCore.Application.Common.Usecases.Restaurant.Categories.Queries;
using HotelCore.Application.Common.Usecases.Restaurant.Products.Queries;

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

    [HttpGet("categories")]
    [Authorize]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(ApiResult.Success(result));
    }

    [HttpGet("products")]
    [Authorize]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProductsListQuery(categoryId), cancellationToken);
        return Ok(ApiResult.Success(result));
    }
}
