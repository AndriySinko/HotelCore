// This file contains code for VerifyCleaningCommand.
using MediatR;

namespace HotelCore.Application.Cleaning.Commands.VerifyCleaning;

public record VerifyCleaningCommand(Guid TaskId) : IRequest;
