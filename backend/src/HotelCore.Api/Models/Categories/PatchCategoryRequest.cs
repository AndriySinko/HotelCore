using Microsoft.AspNetCore.Http;

namespace HotelCore.Api.Models.Categories;

public sealed class PatchCategoryRequest
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public bool RemoveParent { get; set; }
    public bool RemoveIcon { get; set; }
    public IFormFile? IconFile { get; set; }
}
