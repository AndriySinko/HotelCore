// This file contains code for CheckInCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Reception.Commands.CheckIn;






public record CheckInCommand(
    Guid ReservationId,
    string IdType,
    string IdNumber,
    DateTime IdExpiry,
    PaymentMethod PaymentMethod,
    Guid? AlternativeRoomId = null
) : IRequest<CheckInResultDto>;





public record CheckInResultDto(
    bool Success,
    string KeyNumber,
    string RoomNumber);
