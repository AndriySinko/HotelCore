using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Requests;

public record UpdateWorkRequestStatusRequest(WorkRequestStatus NewStatus);
