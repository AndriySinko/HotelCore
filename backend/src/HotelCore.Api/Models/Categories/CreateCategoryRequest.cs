using Microsoft.AspNetCore.Http;

namespace HotelCore.Api.Models.Categories;

public sealed class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public IFormFile? IconFile { get; set; }
}
