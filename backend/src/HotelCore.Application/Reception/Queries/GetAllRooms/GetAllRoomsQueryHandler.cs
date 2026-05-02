// This file contains code for GetAllRoomsQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.Reception;

namespace HotelCore.Application.Reception.Queries.GetAllRooms;

public class GetAllRoomsQueryHandler(IRoomRepository roomRepo)
    : IRequestHandler<GetAllRoomsQuery, List<RoomSummaryDto>>
{
    public async Task<List<RoomSummaryDto>> Handle(GetAllRoomsQuery query, CancellationToken ct)
    {
        var rooms = await roomRepo.GetAllAsync(ct);
        return rooms.Select(r => new RoomSummaryDto(
            r.Id, r.RoomNumber, r.RoomType, r.Floor, r.PricePerNight, r.Status)).ToList();
    }
}
