using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using HotelCore.Api.Helpers;
using HotelCore.Application.MastersProjects.Commands.CreateMasterProject;
using HotelCore.Application.MastersProjects.Commands.DeleteMasterProject;
using HotelCore.Application.MastersProjects.Commands.HideMasterProject;
using HotelCore.Application.MastersProjects.Commands.UnhideMasterProject;
using HotelCore.Application.MastersProjects.Commands.UpdateMasterProject;
using HotelCore.Application.MastersProjects.Models;
using HotelCore.Application.MastersProjects.Queries.GetMasterProjectById;
using HotelCore.Application.MastersProjects.Queries.GetMasterProjectList;
using HotelCore.Application.MastersProjects.Requests;
using HotelCore.Domain.Enums;

namespace HotelCore.Api.Controllers;

public class MastersProjectsController(IMediator mediator) : ApiControllerBase
{
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Worker))]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<IActionResult> Create(
        [FromForm] CreateMasterProjectsRequest createMasterProjectsRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CreateMasterProjectCommand(
            WorkerProfileId: createMasterProjectsRequest.WorkerProfileId,
            Title: createMasterProjectsRequest.Title,
            Description: createMasterProjectsRequest.Description,
            CompletionDate: createMasterProjectsRequest.CompletionDate,
            Images: createMasterProjectsRequest.Images ?? []), cancellationToken);

        return OkResult(result);
    }

    [HttpGet("{masterProjectId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid masterProjectId,
        CancellationToken cancellationToken = default)
    {
        var masterProjectIResult = await mediator.Send(
            new GetMasterProjectByIdQuery(masterProjectId),
            cancellationToken);

        return OkResult(masterProjectIResult);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetList(
        [FromQuery] GetMasterProjectListRequest getMasterProjectListRequest,
        CancellationToken cancellationToken = default)
    {
        var filter = new MasterProjectFilter(
            WorkerProfileId: getMasterProjectListRequest.WorkerProfileId,
            Search: getMasterProjectListRequest.Search);

        var result = await mediator.Send(
            new GetMasterProjectListQuery(filter, getMasterProjectListRequest),
            cancellationToken);

        return OkResult(result);
    }

    [HttpPut]
    [Authorize(Roles = nameof(UserRole.Worker))]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<IActionResult> Update(
        [FromForm] UpdateMasterProjectCommand updateMasterProjectCommand,
        CancellationToken cancellationToken)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        updateMasterProjectCommand = updateMasterProjectCommand with { CurrentUserId = currentUserId };

        await mediator.Send(updateMasterProjectCommand, cancellationToken);
        return Ok();
    }
    
    [HttpDelete]
    [Authorize(Roles = nameof(UserRole.Worker))]
    public async Task<IActionResult> Delete(
        [FromQuery] Guid masterProjectId,
        CancellationToken cancellationToken = default)
    {
        
        await mediator.Send(new DeleteMasterProjectCommand(masterProjectId), cancellationToken);
        return OkResult(new { message = "Master project deleted successfully" });
    }
    
    [HttpPost("{masterProjectId:guid}/hide")]
    [Authorize(Roles = nameof(UserRole.Worker))]
    public async Task<IActionResult> Hide(
        [FromRoute] Guid masterProjectId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        await mediator.Send(new HideMasterProjectCommand(masterProjectId, currentUserId), cancellationToken);

        return OkResult(new { message = "Master Project hidden successfully" });
    }
    
    [HttpPost("{masterProjectId:guid}/unhide")]
    [Authorize(Roles = nameof(UserRole.Worker))]
    public async Task<IActionResult> Unhide(
        [FromRoute] Guid masterProjectId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = UserHelper.GetUserIdOrThrow(HttpContext);
        await mediator.Send(new UnhideMasterProjectCommand(masterProjectId, currentUserId), cancellationToken);

        return OkResult(new { message = "Master Project unhidden successfully" });
    }
}