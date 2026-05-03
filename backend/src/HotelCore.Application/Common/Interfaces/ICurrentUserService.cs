// This file contains code for ICurrentUserService.
namespace HotelCore.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
}
