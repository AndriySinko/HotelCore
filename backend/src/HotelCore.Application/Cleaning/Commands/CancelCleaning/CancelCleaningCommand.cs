// This file contains code for CancelCleaningCommand.
using MediatR;

namespace HotelCore.Application.Cleaning.Commands.CancelCleaning;

public record CancelCleaningCommand(Guid TaskId, string CancellationReason) : IRequest;
