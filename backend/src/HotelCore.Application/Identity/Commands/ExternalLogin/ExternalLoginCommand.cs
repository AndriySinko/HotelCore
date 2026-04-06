using MediatR;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Identity.DTOs;

namespace HotelCore.Application.Identity.Commands.ExternalLogin;

public record ExternalLoginCommand(ExternalUserInfo UserInfo) : IRequest<AuthenticationResult>;
