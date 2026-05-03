// This file contains code for SaveDraftCommand.
using MediatR;

namespace HotelCore.Application.StaffManagement.Commands.SaveDraft;

public record SaveDraftCommand(Guid ScheduleId) : IRequest;
