// This file contains code for UnauthorizedException.
using HotelCore.Domain.Constants;

namespace HotelCore.Domain.Exceptions;

public class UnauthorizedException(string message = "Unauthorized access.", string? code = null) 
    : DomainException(message)
{
    public override string Code => code ?? ErrorCodes.Unauthorized;
}
