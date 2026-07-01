using BarSystem.Domain.Core.Interfaces;

namespace BarSystem.Infra.Core.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _uploadPath;

    public LocalStorageService(string uploadPath)
    {
        _uploadPath = uploadPath;
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(contentType.ToLower()))
            throw new InvalidOperationException($"Tipo de arquivo não suportado: {contentType}");

        if (fileStream.Length > 2 * 1024 * 1024)
            throw new InvalidOperationException("Imagem deve ter no máximo 2MB");

        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        
        var subFolder = DateTime.UtcNow.ToString("yyyy/MM");
        var folderPath = Path.Combine(_uploadPath, subFolder);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, uniqueFileName);

        using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOut, ct);
        }

        return $"/uploads/{subFolder}/{uniqueFileName}";
    }

    public Task DeleteImageAsync(string imageUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Task.CompletedTask;

        var relativePath = imageUrl.TrimStart('/');
        var basePath = Path.GetDirectoryName(_uploadPath.TrimEnd('/')) ?? _uploadPath;
        var filePath = Path.Combine(basePath, relativePath);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
