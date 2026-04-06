using MediatR;
using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Application.EmailVerification.Commands.SendEmailVerification;

public sealed record SendEmailVerificationCommand(
    string Email,
    string? UserId,
    string? BaseUrl) : IRequest<EmailVerificationSendResult>;
