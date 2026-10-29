using Microsoft.AspNetCore.Hosting;

namespace StudentManagementSystem.Services;

public class StudentPhotoService
{
    private readonly IWebHostEnvironment _environment;

    public StudentPhotoService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> SaveAsync(
        IFormFile? photo,
        CancellationToken cancellationToken = default)
    {
        if (photo == null || photo.Length == 0)
            return null;

        if (photo.Length > 2 * 1024 * 1024)
            throw new InvalidOperationException(
                "The photo must not exceed 2 MB.");

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                "Upload a JPG, PNG or WebP image.");

        var folder = Path.Combine(
            _environment.WebRootPath, "uploads", "students");

        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew);

        await photo.CopyToAsync(stream, cancellationToken);

        return $"/uploads/students/{fileName}";
    }
}
