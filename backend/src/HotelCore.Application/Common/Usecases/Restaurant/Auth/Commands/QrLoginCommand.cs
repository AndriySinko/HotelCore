using MediatR;
using HotelCore.Application.Common.Models;

namespace HotelCore.Application.Common.Usecases.Restaurant.Auth.Commands;

public record QrLoginCommand(string QrToken) : IRequest<QrLoginResult>;
