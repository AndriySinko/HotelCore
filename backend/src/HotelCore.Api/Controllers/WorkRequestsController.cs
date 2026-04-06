using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Helpers;
using HotelCore.Application.Communication.WorkRequests.Commands.CreateWorkRequest;
using HotelCore.Application.Communication.WorkRequests.Commands.DeleteWorkRequest;
using HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequest;
using HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequestStatus;
using HotelCore.Application.Communication.WorkRequests.Models;
using HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestById;
using HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestsList;
using HotelCore.Application.Communication.WorkRequests.Requests;
using HotelCore.Domain.Enums;

namespace HotelCore.Api.Controllers;

public class WorkRequestsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{workRequestId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid workRequestId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);
        var workRequest = await mediator.Send(
            new GetWorkRequestByIdQuery(workRequestId, currentUserId),
            cancellationToken);
        return OkResult(workRequest);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetList(
        [FromQuery] GetWorkRequestsListRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);

        var filter = new WorkRequestsFilter(
            SeekerUserId: request.SeekerUserId,
            LocationId: request.LocationId,
            PreferredDate: request.PreferredDate,
            MinBudget: request.MinBudget,
            MaxBudget: request.MaxBudget,
            CategoryIds: request.CategoryIds,
            Status: request.Status,
            ExcludeStatus: null);
       
        var result = await mediator.Send(
            new GetWorkRequestsListQuery(filter, request, currentUserId),
            cancellationToken);

        return OkResult(result);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Seeker))]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var workRequestId = await mediator.Send(new CreateWorkRequestCommand(
            Title: request.Title,
            Description: request.Description,
            SeekerProfileId: request.SeekerProfileId,
            CategoryId: request.CategoryId,
            LocationId: request.LocationId,
            Budget: request.Budget,
            PreferredDate: request.PreferredDate,
            Tags: request.Tags ?? []), cancellationToken);

        var currentUserId = UserHelper.GetUserIdGuid(HttpContext);
        var workRequest = await mediator.Send(
            new GetWorkRequestByIdQuery(workRequestId, currentUserId),
            cancellationToken);

        return CreatedResult(nameof(GetById), new { workRequestId }, workRequest);
    }

    [HttpPut("{workRequestId:guid}")]
    [Authorize(Roles = nameof(UserRole.Seeker))]
    public async Task<IActionResult> Update(
        [FromRoute] Guid workRequestId,
        [FromBody] UpdateWorkRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);

        await mediator.Send(new UpdateWorkRequestCommand(
            WorkRequestId: workRequestId,
            CurrentUserId: currentUserId,
            Title: request.Title,
            Description: request.Description,
            CategoryId: request.CategoryId,
            LocationId: request.LocationId,
            Budget: request.Budget,
            PreferredDate: request.PreferredDate,
            Tags: request.Tags), cancellationToken);

        var workRequest = await mediator.Send(
            new GetWorkRequestByIdQuery(workRequestId, currentUserId),
            cancellationToken);

        return OkResult(workRequest);
    }

    [HttpPatch("{workRequestId:guid}/status")]
    [Authorize(Roles = nameof(UserRole.Seeker))]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid workRequestId,
        [FromBody] UpdateWorkRequestStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        
        await mediator.Send(
            new UpdateWorkRequestStatusCommand(workRequestId, request.NewStatus, currentUserId),
            cancellationToken);

        return OkResult(new { message = "Work request status updated successfully" });
    }

    [HttpDelete("{workRequestId:guid}")]
    [Authorize(Roles = nameof(UserRole.Seeker))]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid workRequestId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        
        await mediator.Send(new DeleteWorkRequestCommand(workRequestId, currentUserId), cancellationToken);

        return OkResult(new { message = "Work request deleted successfully" });
    }
}
