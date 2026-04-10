// This file contains code for CreateWorkRequestRequest.
namespace HotelCore.Application.Communication.WorkRequests.Requests;

public record CreateWorkRequestRequest(
    string Title,
    string Description,
    Guid SeekerProfileId,
    Guid CategoryId,
    Guid LocationId,
    decimal? Budget = null,
    DateTime? PreferredDate = null,
    string[]? Tags = null);
