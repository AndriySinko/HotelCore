using MediatR;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Identity.Commands.Login;

public record LoginCommand(
    string Email, 
    string Password) 
    : IRequest<AuthenticationResult>;
