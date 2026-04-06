using MediatR;
using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Application.EmailVerification.Commands.VerifyEmailVerification;

public sealed record VerifyEmailVerificationCommand(
    string Email,
    string? Code,
    string? Token) : IRequest<EmailVerificationResult>;
