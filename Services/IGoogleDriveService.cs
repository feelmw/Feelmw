using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public interface IGoogleDriveService
{
    Task<IReadOnlyList<DriveFolder>> ListFoldersAsync(string? searchTerm = null, CancellationToken cancellationToken = default);

    Task<DriveSaveResult> UploadFileAsync(
        string folderId,
        string folderName,
        string fileName,
        string contentType,
        byte[] content,
        CancellationToken cancellationToken = default);
}
