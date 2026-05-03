using HotelCore.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using QRCoder;

namespace HotelCore.Infrastructure.Services;

public class QrCodeService(IWebHostEnvironment env) : IQrCodeService
{
    public async Task<string> GenerateAsync(string data, string fileName, CancellationToken ct = default)
    {
        var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var dir = Path.Combine(webRoot, "qrcodes");
        Directory.CreateDirectory(dir);

        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        var pngBytes = qrCode.GetGraphic(20);

        var filePath = Path.Combine(dir, $"{fileName}.png");
        await File.WriteAllBytesAsync(filePath, pngBytes, ct);

        return $"/qrcodes/{fileName}.png";
    }
}
