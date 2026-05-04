namespace HotelCore.Application.Common.Models;

public record QrLoginResult(
    bool Succeeded,
    string? Token,
    string? GuestId,
    string? Name,
    string? RoomNumber,
    string? Error
)
{
    public static QrLoginResult Success(string token, string guestId, string name, string? roomNumber) =>
        new(true, token, guestId, name, roomNumber, null);

    public static QrLoginResult Failure(string error) =>
        new(false, null, null, null, null, error);
}
