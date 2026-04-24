using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.Common.Usecases.Restaurant.Auth.Commands;
using HotelCore.Application.Common.Usecases.Restaurant.Categories.Queries;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.Commands;
using HotelCore.Application.Common.Usecases.Restaurant.Orders.Queries;
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

    [HttpPost("orders")]
    [Authorize]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/restaurant/orders/{result.Id}", ApiResult.Success(result));
    }

    [HttpGet("orders/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetOrderQuery(id), cancellationToken);
        return Ok(ApiResult.Success(result));
    }

    [HttpDelete("orders/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CancelOrderCommand(id), cancellationToken);
        return Ok(ApiResult.Success(result));
    }
}
