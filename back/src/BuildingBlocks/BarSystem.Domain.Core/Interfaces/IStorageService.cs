namespace BarSystem.Domain.Core.Interfaces;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteImageAsync(string imageUrl, CancellationToken ct = default);
}
