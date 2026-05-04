using MediatR;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Common.Usecases.Restaurant.Auth.Commands;
using HotelCore.Application.Identity;

namespace HotelCore.Application.Common.Usecases.Restaurant.Auth.CommandHandlers;

public class QrLoginHandler(IIdentityService identityService)
    : IRequestHandler<QrLoginCommand, QrLoginResult>
{
    public Task<QrLoginResult> Handle(QrLoginCommand request, CancellationToken cancellationToken) =>
        identityService.QrLoginAsync(request.QrToken, cancellationToken);
}
