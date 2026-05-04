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
    /// <summary>
    /// Authenticates a guest via the QR code scanned from their table or room card.
    /// On success, returns a JWT used for all subsequent restaurant endpoints.
    /// The QR may encode a full URL or a bare reservation code (HC-XXXXXX).
    /// </summary>
    [HttpPost("auth/qr")]
    [AllowAnonymous]  // no token exists yet — this endpoint IS the authentication step
    public async Task<IActionResult> QrLogin(
        [FromBody] QrLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Succeeded)
            return Unauthorized(ApiResult.Failure(result.Error ?? "Authentication failed."));

        return Ok(ApiResult.Success(result));
    }

    /// <summary>Returns all menu categories for building the category filter bar.</summary>
    [HttpGet("categories")]
    [Authorize]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(ApiResult.Success(result));
    }

    /// <summary>
    /// Returns menu items. Pass <c>categoryId</c> to filter to a single section;
    /// omit it to get the full menu.
    /// </summary>
    [HttpGet("products")]
    [Authorize]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProductsListQuery(categoryId), cancellationToken);
        return Ok(ApiResult.Success(result));
    }

    /// <summary>
    /// Places a new room-service order for the authenticated guest.
    /// Returns 201 with the order resource location.
    /// </summary>
    [HttpPost("orders")]
    [Authorize]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/restaurant/orders/{result.Id}", ApiResult.Success(result));
    }

    /// <summary>Returns the current state of an order. Used by the mobile app for live status polling.</summary>
    [HttpGet("orders/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetOrderQuery(id), cancellationToken);
        return Ok(ApiResult.Success(result));
    }

    /// <summary>
    /// Cancels an order. Only succeeds while the order is still in <c>received</c> status —
    /// once the kitchen starts preparing it, the request is rejected with 400.
    /// </summary>
    [HttpDelete("orders/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CancelOrderCommand(id), cancellationToken);
        return Ok(ApiResult.Success(result));
    }
}
