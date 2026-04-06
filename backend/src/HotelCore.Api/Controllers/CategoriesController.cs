using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models.Categories;
using HotelCore.Application.Categories.Commands.CreateCategory;
using HotelCore.Application.Categories.Commands.DeleteCagtegory;
using HotelCore.Application.Categories.Commands.PatchCategory;
using HotelCore.Application.Categories.Commands.RestoreCategory;
using HotelCore.Application.Categories.Commands.UpdateCategory;
using HotelCore.Application.Categories.Queries.GetCategories;
using HotelCore.Application.Categories.Queries.GetCategoryById;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Api.Controllers;

public class CategoriesController(IMediator mediator) : ApiControllerBase 
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromQuery] Guid? parentId,
        [FromQuery] string mode = "direct",
        CancellationToken cancellationToken = default)
    {
        var parsedMode = mode.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? CategoryLoadMode.All
            : CategoryLoadMode.Direct;

        var data = await mediator.Send(
            new GetCategoriesQuery(parentId, parsedMode, OverpassIsDeleteFilter: false),
            cancellationToken);
        
        return OkResult(data);
    }
    
    [HttpGet("admin")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> GetForAdmin(
        [FromQuery] Guid? parentId,
        [FromQuery] string mode = "direct",
        CancellationToken cancellationToken = default)
    {
        var parsedMode = mode.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? CategoryLoadMode.All
            : CategoryLoadMode.Direct;

        var data = await mediator.Send(
            new GetCategoriesQuery(parentId, parsedMode, OverpassIsDeleteFilter: true),
            cancellationToken);

        return OkResult(data);
    }

    [HttpGet("{categoryId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid categoryId,
        [FromQuery] string mode = "direct",
        [FromQuery] bool overpassIsDeleteFilter = false,
        CancellationToken cancellationToken = default)
    {
        var parsedMode = mode.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? CategoryLoadMode.All
            : CategoryLoadMode.Direct;

        var data = await mediator.Send(
            new GetCategoryByIdQuery(categoryId, parsedMode, overpassIsDeleteFilter), 
            cancellationToken);
        
        return data is null
            ? NotFoundResult("Category not found")
            : OkResult(data);
    }
    
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Create(
        [FromForm] CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var categoryCommand = new CreateCategoryCommand(
            request.Name,
            request.Slug,
            request.Description,
            request.IconFile,
            request.ParentId);

        var created = await mediator.Send(
            categoryCommand, 
            cancellationToken);
        
        return OkResult(created);
    }
    
    [HttpPost("{categoryId:guid}/restore")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Restore(
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(
            new RestoreCategoryCommand(categoryId),
            cancellationToken);

        return OkResult(
            new
            {
                message = "Restored"
            });
    }
    
    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Update(
        [FromRoute] Guid categoryId, 
        [FromBody] UpdateCategoryCommand categoryCommand, 
        CancellationToken cancellationToken = default)
    {
        if (categoryId != categoryCommand.Id)
            throw new BadRequestException("Route id != body id");
        
        var updated = await mediator.Send(
            categoryCommand, 
            cancellationToken);
        
        return OkResult(updated);
    }

    [HttpPatch("{categoryId:guid}")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Patch(
        [FromRoute] Guid categoryId,
        [FromForm] PatchCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new PatchCategoryCommand(
            categoryId,
            request.Name,
            request.Slug,
            request.Description,
            request.ParentId,
            request.RemoveParent,
            request.RemoveIcon,
            request.IconFile);

        var updated = await mediator.Send(command, cancellationToken);

        return OkResult(updated);
    }

    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid categoryId, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(
            new DeleteCategoryCommand(categoryId), 
            cancellationToken);
        
        return OkResult(
            new
            {
                message = "Deleted"
            });
    }
}
