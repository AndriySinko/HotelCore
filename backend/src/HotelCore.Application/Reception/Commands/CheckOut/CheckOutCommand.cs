// This file contains code for CheckOutCommand.
using MediatR;

namespace HotelCore.Application.Reception.Commands.CheckOut;





public record CheckOutCommand(Guid ReservationId) : IRequest<CheckOutResultDto>;





public record CheckOutResultDto(
    bool Success,
    string GuestName,
    decimal TotalCharged,
    string RoomNumber);
