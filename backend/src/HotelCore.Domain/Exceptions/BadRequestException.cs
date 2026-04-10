// This file contains code for BadRequestException.
using HotelCore.Domain.Constants;

namespace HotelCore.Domain.Exceptions;

public class BadRequestException(string message, string? code = null) 
    : DomainException(message)
{
    public override string Code => code ?? ErrorCodes.BadRequest;
}
