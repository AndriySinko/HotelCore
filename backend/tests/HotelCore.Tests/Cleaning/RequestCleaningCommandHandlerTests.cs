

using FluentAssertions;
using Moq;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Cleaning;
using HotelCore.Application.Cleaning.Commands.RequestCleaning;
using HotelCore.Domain.Entities.Cleaning;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using Xunit;

namespace HotelCore.Tests.Cleaning;

public class RequestCleaningCommandHandlerTests
{
    private readonly Mock<ICleaningTaskRepository> _cleaningTaskRepository = new();
    private readonly Mock<IUnitOfWork>             _unitOfWork             = new();

    private RequestCleaningCommandHandler CreateHandler() =>
        new(_cleaningTaskRepository.Object, _unitOfWork.Object);

    private static RequestCleaningCommand ValidCommand(Guid roomId) => new()
    {
        RoomId        = roomId,
        RequestType   = CleaningRequestType.GuestRequest,
        ScheduledDate = DateTime.UtcNow.Date,
        Priority      = 3,
    };

    [Fact]
    public async Task Handle_NoExistingActiveTasks_CreatesTaskAndReturnsId()
    {
        
        var roomId = Guid.NewGuid();
        _cleaningTaskRepository
            .Setup(repository => repository.GetByRoomAsync(roomId, default))
            .ReturnsAsync(new List<CleaningTask>());
        _cleaningTaskRepository
            .Setup(repository => repository.AddAsync(It.IsAny<CleaningTask>(), default))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(default)).ReturnsAsync(1);

        
        var resultId = await CreateHandler().Handle(ValidCommand(roomId), CancellationToken.None);

        
        resultId.Should().NotBe(Guid.Empty);
        _cleaningTaskRepository.Verify(repository => repository.AddAsync(It.IsAny<CleaningTask>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_ActiveTaskAlreadyExists_ThrowsConflictException()
    {
        
        var roomId = Guid.NewGuid();
        var existingActiveTask = new CleaningTask { RoomId = roomId };
        existingActiveTask.SetStatus(CleaningTaskStatus.Assigned);

        _cleaningTaskRepository
            .Setup(repository => repository.GetByRoomAsync(roomId, default))
            .ReturnsAsync(new List<CleaningTask> { existingActiveTask });

        
        var act = () => CreateHandler().Handle(ValidCommand(roomId), CancellationToken.None);

        
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*active cleaning task*");
    }

    [Fact]
    public async Task Handle_CompletedTaskExists_AllowsNewRequest()
    {
        
        var roomId = Guid.NewGuid();
        var completedTask = new CleaningTask { RoomId = roomId };
        completedTask.SetStatus(CleaningTaskStatus.Completed);

        _cleaningTaskRepository
            .Setup(repository => repository.GetByRoomAsync(roomId, default))
            .ReturnsAsync(new List<CleaningTask> { completedTask });
        _cleaningTaskRepository
            .Setup(repository => repository.AddAsync(It.IsAny<CleaningTask>(), default))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(default)).ReturnsAsync(1);

        
        var act = () => CreateHandler().Handle(ValidCommand(roomId), CancellationToken.None);

        
        await act.Should().NotThrowAsync();
    }
}
