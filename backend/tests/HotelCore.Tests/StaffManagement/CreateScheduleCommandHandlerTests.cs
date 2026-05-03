
using FluentAssertions;
using Moq;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.StaffManagement;
using HotelCore.Application.StaffManagement.Commands.CreateSchedule;
using HotelCore.Domain.Entities.StaffManagement;
using HotelCore.Domain.Exceptions;
using Xunit;

namespace HotelCore.Tests.StaffManagement;

public class CreateScheduleCommandHandlerTests
{
    private readonly Mock<IWorkScheduleRepository> _scheduleRepo = new();
    private readonly Mock<IUnitOfWork>             _unitOfWork   = new();

    private CreateScheduleCommandHandler CreateHandler() =>
        new(_scheduleRepo.Object, _unitOfWork.Object);

    private static CreateScheduleCommand MakeCommand(DateTime start, DateTime end) => new(
        PeriodStart:     start,
        PeriodEnd:       end,
        CreatedByUserId: Guid.NewGuid()
    );

    [Fact]
    public async Task Handle_NoOverlap_CreatesScheduleAndReturnsId()
    {
        var start = new DateTime(2026, 5, 1);
        var end   = new DateTime(2026, 5, 7);

        _scheduleRepo.Setup(r => r.GetByPeriodAsync(start, end, default))
            .ReturnsAsync(new List<WorkSchedule>());
        _scheduleRepo.Setup(r => r.AddAsync(It.IsAny<WorkSchedule>(), default))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await CreateHandler().Handle(MakeCommand(start, end), CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_OverlappingScheduleExists_ThrowsConflictException()
    {
        var start = new DateTime(2026, 5, 1);
        var end   = new DateTime(2026, 5, 7);

        var existing = new WorkSchedule { PeriodStart = start, PeriodEnd = end };
        _scheduleRepo.Setup(r => r.GetByPeriodAsync(start, end, default))
            .ReturnsAsync(new List<WorkSchedule> { existing });

        var act = () => CreateHandler().Handle(MakeCommand(start, end), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_EndBeforeStart_ThrowsBadRequestException()
    {
        var start = new DateTime(2026, 5, 7);
        var end   = new DateTime(2026, 5, 1); 

        _scheduleRepo.Setup(r => r.GetByPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), default))
            .ReturnsAsync(new List<WorkSchedule>());

        var act = () => CreateHandler().Handle(MakeCommand(start, end), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
    }
}
