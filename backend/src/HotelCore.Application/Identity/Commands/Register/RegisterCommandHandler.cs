using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HotelCore.Application.Identity.DTOs;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Models;
using HotelCore.Application.EmailVerification.Options;

namespace HotelCore.Application.Identity.Commands.Register;

public class RegisterCommandHandler(
    IIdentityService identityService,
    IVerificationCodeGenerator codeGenerator,
    IEmailVerificationStore verificationStore,
    IEventProducer eventProducer,
    IOptions<EmailVerificationOptions> options,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    public async Task<AuthenticationResult> Handle(
        RegisterCommand request, 
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing registration request for {Email}", request.Email);
        
        var result = await identityService.RegisterAsync(new RegisterUserDto(
            request.Email, 
            request.Password, 
            request.FirstName, 
            request.LastName,
            request.Role));

        if (result.Succeeded)
        {
            logger.LogInformation("User {Email} registered successfully", request.Email);

            // Generate Verification Code & Token
            var code = codeGenerator.Generate(6);
            var token = Guid.NewGuid().ToString("N");

            // Save to store (Redis/DB)
            var record = new EmailVerificationRecord
            {
                Email = request.Email,
                UserId = result.UserId,
                Code = code,
                Token = token,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(options.Value.TimeToLiveHours),
                Attempts = 0
            };

            await verificationStore.SaveAsync(record, TimeSpan.FromHours(options.Value.TimeToLiveHours), cancellationToken);

            logger.LogInformation("Verification code generated and event published for {Email}", request.Email);
        }
        else
        {
            logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, result.Error);
        }

        return result;
    }
}
