using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Users.Commands.CreateUser;
using HotelCore.Application.Users.Queries.GetCurrentUser;
using HotelCore.Application.Users.Queries.GetUserById;
using HotelCore.Application.Users.Queries.GetUsersList;
using HotelCore.Application.Users.Queries.GetPublicProfile;
using HotelCore.Domain.Enums;

namespace HotelCore.Api.Controllers;

[Authorize(Roles = nameof(UserRole.Administrator))]
public class UsersController(IMediator mediator) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return OkResult(result);
    }

    [HttpGet("me")]
    [Authorize] // Overrides the controller-level Administrator role requirement
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        
        return result is null 
            ? NotFoundResult("User not found") 
            : OkResult(result);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetUserByIdQuery(userId), 
            cancellationToken);
        
        return result is null 
            ? NotFoundResult("User not found") 
            : OkResult(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetList(
        [FromQuery] PageRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetUsersListQuery(request.Page, request.PageSize), 
            cancellationToken);
        
        return OkResult(result);
    }

    [HttpGet("{userId:guid}/public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProfile(
        [FromRoute] Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetPublicProfileQuery(userId), 
            cancellationToken);
        
        return result is null 
            ? NotFoundResult("User not found") 
            : OkResult(result);
    }
}