// This file contains code for WorkRequestsFilter.
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Models;

public record WorkRequestsFilter(
    Guid? SeekerProfileId = null,
    Guid? SeekerUserId = null,
    Guid? LocationId = null,
    DateTime? PreferredDate = null,
    decimal? MinBudget = null,
    decimal? MaxBudget = null,
    IReadOnlyCollection<Guid>? CategoryIds = null,
    WorkRequestStatus? Status = null,
    WorkRequestStatus? ExcludeStatus = null);
