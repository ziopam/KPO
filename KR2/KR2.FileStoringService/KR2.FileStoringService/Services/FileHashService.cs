using System.Security.Cryptography;

namespace KR2.FileStoringService.Services;

public class FileHashService
{
    public async Task<string> CalculateHashAsync(Stream fileStream)
    {
        fileStream.Position = 0;
        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(fileStream);
        fileStream.Position = 0;

        var base64 = Convert.ToBase64String(hashBytes);
        return base64
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "");
    }
}
