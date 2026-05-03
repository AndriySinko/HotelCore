// This file contains code for GetAllRoomsQuery.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Reception.Queries.GetAllRooms;

public record GetAllRoomsQuery : IRequest<List<RoomSummaryDto>>;

public record RoomSummaryDto(
    Guid Id,
    string RoomNumber,
    RoomType RoomType,
    int Floor,
    decimal PricePerNight,
    RoomStatus Status);
