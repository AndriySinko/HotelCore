// This file contains code for CompleteCleaningCommand.
using MediatR;

namespace HotelCore.Application.Cleaning.Commands.CompleteCleaning;

public record CompleteCleaningCommand(Guid TaskId) : IRequest;
