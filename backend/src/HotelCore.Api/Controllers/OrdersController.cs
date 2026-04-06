using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using HotelCore.Api.Common;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Orders.Commands.AddOrderImages;
using HotelCore.Application.Orders.Commands.ChangeOrderStatus;
using HotelCore.Application.Orders.Commands.CreateOrder;
using HotelCore.Application.Orders.Commands.CreateSubOrder;
using HotelCore.Application.Orders.Commands.HideOrder;
using HotelCore.Application.Orders.Commands.UnhideOrder;
using HotelCore.Application.Orders.Commands.UpdateOrder;
using HotelCore.Application.Orders.Models;
using HotelCore.Application.Orders.Queries.GetOrderById;
using HotelCore.Application.Orders.Queries.GetOrderImages;
using HotelCore.Application.Orders.Queries.GetOrdersList;
using HotelCore.Application.Orders.Queries.GetSubOrders;
using HotelCore.Application.Orders.Requests;

using HotelCore.Api.Helpers;
using HotelCore.Domain.Enums;

namespace HotelCore.Api.Controllers;

public class OrdersController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{orderId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);
        var guestToken = Request.Headers[RequestHeaderConstants.GUEST_TOKEN_KEY].FirstOrDefault();

        var order = await mediator.Send(
            new GetOrderByIdQuery(orderId, currentUserId, guestToken), 
            cancellationToken);

        return OkResult(order);
    }

    [HttpGet("{orderId:guid}/images")]
    [AllowAnonymous]
    public async Task<IActionResult> GetImages(
        [FromRoute] Guid orderId,
        [FromQuery] PageRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);
        var guestToken = Request.Headers[RequestHeaderConstants.GUEST_TOKEN_KEY].FirstOrDefault();
        
        var result = await mediator.Send(
            new GetOrderImagesQuery(orderId, pagination, currentUserId, guestToken), 
            cancellationToken);

        return OkResult(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetList(
        [FromQuery] GetOrdersListRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);

        var filter = new OrdersFilter(
            CreatedByUserId: request.CreatedByUserId,
            ClientUserId: request.ClientUserId,
            GuestAccessToken: request.GuestAccessToken,
            Status: request.Status,
            ExcludeStatus: null, // Logic handled in QueryHandler
            IsHidden: request.IsHidden,
            SortBy: request.SortBy,
            SortDescending: request.SortDescending);

        var result = await mediator.Send(
            new GetOrdersListQuery(filter, request, currentUserId),
            cancellationToken);

        return OkResult(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);

        var orderId = await mediator.Send(new CreateOrderCommand(
            Title: request.Title,
            Description: request.Description,
            Price: request.Price,
            PaymentType: request.PaymentType,
            ClientPhoneNumber: request.ClientPhoneNumber,
            ClientEmail: request.ClientEmail,
            ClientUserId: request.ClientUserId,
            CreatedByUserId: currentUserId), cancellationToken);

        var order = await mediator.Send(
            new GetOrderByIdQuery(orderId, currentUserId), 
            cancellationToken);

        return CreatedResult(nameof(GetById), new { orderId }, order);
    }

    [HttpPut("{orderId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(
        [FromRoute] Guid orderId,
        [FromBody] UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);

        await mediator.Send(new UpdateOrderCommand(
            OrderId: orderId,
            CurrentUserId: currentUserId,
            Title: request.Title,
            Description: request.Description,
            Price: request.Price,
            PaymentType: request.PaymentType,
            ClientPhoneNumber: request.ClientPhoneNumber,
            ClientEmail: request.ClientEmail,
            ClientUserId: request.ClientUserId), cancellationToken);

        var order = await mediator.Send(
            new GetOrderByIdQuery(orderId, currentUserId), 
            cancellationToken);

        return OkResult(order);
    }

    [HttpPost("{orderId:guid}/hide")]
    [Authorize]
    public async Task<IActionResult> Hide(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        await mediator.Send(new HideOrderCommand(orderId, currentUserId), cancellationToken);

        return OkResult(new { message = "Order hidden successfully" });
    }

    [HttpPost("{orderId:guid}/unhide")]
    [Authorize]
    public async Task<IActionResult> Unhide(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        await mediator.Send(new UnhideOrderCommand(orderId, currentUserId), cancellationToken);

        return OkResult(new { message = "Order unhidden successfully" });
    }

    [HttpPatch("{orderId:guid}/status")]
    [Authorize]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] Guid orderId,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        await mediator.Send(
            new ChangeOrderStatusCommand(orderId, request.NewStatus, currentUserId),
            cancellationToken);

        return OkResult(new { message = "Order status updated successfully" });
    }

    [HttpPost("{orderId:guid}/images")]
    [Authorize]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<IActionResult> AddImages(
        [FromRoute] Guid orderId,
        [FromForm] AddOrderImagesRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        await mediator.Send(
            new AddOrderImagesCommand(orderId, currentUserId, request.Images),
            cancellationToken);

        return OkResult(new { message = "Images added successfully" });
    }

    [HttpGet("{orderId:guid}/sub-orders")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubOrders(
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);
        var guestToken = Request.Headers[RequestHeaderConstants.GUEST_TOKEN_KEY].FirstOrDefault();

        var result = await mediator.Send(
            new GetSubOrdersQuery(orderId, currentUserId, guestToken),
            cancellationToken);

        return OkResult(result);
    }

    [HttpPost("{orderId:guid}/sub-orders")]
    [Authorize]
    public async Task<IActionResult> CreateSubOrder(
        [FromRoute] Guid orderId,
        [FromBody] CreateSubOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);

        var subOrderId = await mediator.Send(new CreateSubOrderCommand(
            ParentOrderId: orderId,
            Title: request.Title,
            Description: request.Description,
            Price: request.Price,
            PaymentType: request.PaymentType,
            CurrentUserId: currentUserId), cancellationToken);

        var subOrder = await mediator.Send(
            new GetOrderByIdQuery(subOrderId, currentUserId),
            cancellationToken);

        return CreatedResult(nameof(GetById), new { orderId = subOrderId }, subOrder);
    }
}
