// This file contains code for UpdateWorkRequestStatusRequest.
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Requests;

public record UpdateWorkRequestStatusRequest(WorkRequestStatus NewStatus);
