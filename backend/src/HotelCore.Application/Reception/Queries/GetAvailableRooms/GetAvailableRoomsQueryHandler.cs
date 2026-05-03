// This file contains code for GetAvailableRoomsQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Reception;

namespace HotelCore.Application.Reception.Queries.GetAvailableRooms;

public class GetAvailableRoomsQueryHandler(IRoomRepository roomRepo)
    : IRequestHandler<GetAvailableRoomsQuery, List<AvailableRoomDto>>
{
    public async Task<List<AvailableRoomDto>> Handle(GetAvailableRoomsQuery query, CancellationToken ct)
    {
        var rooms = await roomRepo.GetAvailableRoomsAsync(query.CheckIn, query.CheckOut, query.Type, ct);

        return rooms.Select(r => new AvailableRoomDto(
            r.Id, r.RoomNumber, r.RoomType, r.Floor, r.PricePerNight)).ToList();
    }
}
