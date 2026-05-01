// This file contains code for IQrCodeService.
namespace HotelCore.Application.Common.Interfaces;

public interface IQrCodeService
{
    Task<string> GenerateAsync(string data, string fileName, CancellationToken ct = default);
}
