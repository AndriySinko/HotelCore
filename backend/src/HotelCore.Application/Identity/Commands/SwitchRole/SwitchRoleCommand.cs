using MediatR;
using HotelCore.Application.Common.Models;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Identity.Commands.SwitchRole;

public record SwitchRoleCommand(
    UserRole TargetRole
) : IRequest<AuthenticationResult>;
