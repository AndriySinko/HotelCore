// This file contains code for UpdateWorkRequestRequest.
namespace HotelCore.Application.Communication.WorkRequests.Requests;

public record UpdateWorkRequestRequest(
    string? Title = null,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? LocationId = null,
    decimal? Budget = null,
    DateTime? PreferredDate = null,
    string[]? Tags = null);
