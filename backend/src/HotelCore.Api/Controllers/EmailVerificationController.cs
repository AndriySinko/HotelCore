using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelCore.Api.Models;
using HotelCore.Application.EmailVerification.Commands.SendEmailVerification;
using HotelCore.Application.EmailVerification.Commands.VerifyEmailVerification;
using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Api.Controllers;

public sealed class EmailVerificationController(IMediator mediator) : ApiControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> Send(
        [FromBody] SendEmailVerificationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequestResult("Email is required", "EMAIL_REQUIRED");
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var result = await mediator.Send(
            new SendEmailVerificationCommand(request.Email, request.UserId, baseUrl),
            cancellationToken);

        return OkResult(new SendEmailVerificationResponse(result.ExpiresAt));
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(
        [FromBody] VerifyEmailVerificationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequestResult("Email is required", "EMAIL_REQUIRED");
        }

        if (string.IsNullOrWhiteSpace(request.Code)
            && string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequestResult("Code or token is required", "CODE_OR_TOKEN_REQUIRED");
        }

        var result = await mediator.Send(
            new VerifyEmailVerificationCommand(request.Email, request.Code, request.Token),
            cancellationToken);

        return MapResult(result);
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> Confirm(
        [FromQuery] string email,
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
             return BadRequestResult("Email and token are required", "EMAIL_AND_TOKEN_REQUIRED");
        }

        var result = await mediator.Send(
            new VerifyEmailVerificationCommand(email, null, token),
            cancellationToken);

        return MapResult(result);
    }

    private IActionResult MapResult(EmailVerificationResult result)
    {
        return result.Status switch
        {
            EmailVerificationStatus.Success => OkResult(new { status = "verified" }),
            EmailVerificationStatus.NotFound => NotFoundResult("Verification request not found"),
            EmailVerificationStatus.Expired => BadRequestResult("Verification code expired", "EXPIRED"),
            EmailVerificationStatus.InvalidCode => BadRequestResult(
                $"Invalid code. Attempts left: {result.AttemptsLeft}", "INVALID_CODE"),
            EmailVerificationStatus.TooManyAttempts => StatusCode(
                StatusCodes.Status429TooManyRequests,
                ApiResult.Failure("Too many requests, the code is expired.", "TOO_MANY_REQUESTS")),
            _ => StatusCode(StatusCodes.Status500InternalServerError, ApiResult.Failure("Internal server error"))
        };
    }
}

public sealed record SendEmailVerificationRequest(string Email, string? UserId);
public sealed record SendEmailVerificationResponse(DateTimeOffset ExpiresAt);
public sealed record VerifyEmailVerificationRequest(string Email, string? Code, string? Token);
