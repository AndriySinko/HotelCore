// This file contains code for GetAvailableRoomsQuery.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Reception.Queries.GetAvailableRooms;

public record GetAvailableRoomsQuery(DateTime CheckIn, DateTime CheckOut, RoomType? Type = null)
    : IRequest<List<AvailableRoomDto>>;

public record AvailableRoomDto(
    Guid Id,
    string RoomNumber,
    RoomType RoomType,
    int Floor,
    decimal PricePerNight);
