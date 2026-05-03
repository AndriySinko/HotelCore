









using FluentAssertions;
using Moq;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Reception;
using HotelCore.Application.Reception.Commands.CheckIn;
using HotelCore.Domain.Entities.Reception;
using HotelCore.Domain.Entities.Users;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using Xunit;

namespace HotelCore.Tests.Reception;

public class CheckInCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepo = new();
    private readonly Mock<IRoomRepository>        _roomRepo        = new();
    private readonly Mock<IGuestRepository>       _guestRepo       = new();
    private readonly Mock<IUnitOfWork>            _unitOfWork      = new();

    private CheckInCommandHandler CreateHandler() =>
        new(_reservationRepo.Object, _roomRepo.Object, _guestRepo.Object, _unitOfWork.Object);

    private static Reservation MakeReservation(Room room, Guest guest)
    {
        var reservation = new Reservation
        {
            GuestId = guest.Id,
            Guest   = guest,
            RoomId  = room.Id,
            Room    = room,
            CheckInDate  = DateTime.UtcNow.Date,
            CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
            NumberOfGuests = 1,
        };
        reservation.SetStatus(ReservationStatus.Confirmed);
        return reservation;
    }

    private static Guest MakeGuest() => new()
    {
        FirstName = "Jane",
        LastName  = "Doe",
        Email     = "jane@example.com",
        UserName  = "jane@example.com",
    };

    private static Room MakeAvailableRoom()
    {
        var room = new Room { RoomNumber = "101", Floor = 1, PricePerNight = 100m };
        room.SetStatus(RoomStatus.Reserved);
        return room;
    }

    [Fact]
    public async Task Handle_ValidCheckIn_ReturnsKeyNumber()
    {
        
        var guest       = MakeGuest();
        var room        = MakeAvailableRoom();
        var reservation = MakeReservation(room, guest);

        _reservationRepo.Setup(r => r.GetByIdWithDetailsAsync(reservation.Id, default))
            .ReturnsAsync(reservation);
        _roomRepo.Setup(r => r.GetByIdAsync(room.Id, default))
            .ReturnsAsync(room);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var command = new CheckInCommand(
            ReservationId:   reservation.Id,
            IdType:          "Passport",
            IdNumber:        "AB123456",
            IdExpiry:        DateTime.UtcNow.AddYears(2),
            PaymentMethod:   PaymentMethod.Card,
            AlternativeRoomId: null);

        
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.KeyNumber.Should().StartWith("K-");
        result.RoomNumber.Should().Be("101");
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ThrowsNotFoundException()
    {
        
        _reservationRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Reservation?)null);

        var command = new CheckInCommand(
            ReservationId:   Guid.NewGuid(),
            IdType:          "Passport",
            IdNumber:        "X",
            IdExpiry:        DateTime.UtcNow.AddYears(1),
            PaymentMethod:   PaymentMethod.Cash,
            AlternativeRoomId: null);

        
        var act = () => CreateHandler().Handle(command, CancellationToken.None);

        
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
