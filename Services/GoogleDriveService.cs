using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FeelmwLogistika.Blazor.Models;

namespace FeelmwLogistika.Blazor.Services;

public sealed class GoogleDriveService(HttpClient httpClient, IGoogleAuthService googleAuthService) : IGoogleDriveService
{
    private const string DriveScopes = "https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/drive.metadata.readonly";
    private const string FolderMimeType = "application/vnd.google-apps.folder";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<IReadOnlyList<DriveFolder>> ListFoldersAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        string token = await googleAuthService.GetRequiredAccessTokenAsync(DriveScopes, cancellationToken);
        string query = $"mimeType = '{FolderMimeType}' and trashed = false";
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query += $" and name contains '{EscapeDriveQuery(searchTerm.Trim())}'";
        }

        string url = "https://www.googleapis.com/drive/v3/files"
            + $"?q={Uri.EscapeDataString(query)}"
            + "&pageSize=100"
            + "&orderBy=name"
            + "&fields=files(id,name,webViewLink)"
            + "&supportsAllDrives=true"
            + "&includeItemsFromAllDrives=true";

        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(DriveError(json, "Drive karpetak ezin izan dira kargatu."));
        }

        DriveFileList? list = JsonSerializer.Deserialize<DriveFileList>(json, JsonOptions);
        return list?.Files
            .Where(file => !string.IsNullOrWhiteSpace(file.Id))
            .Select(file => new DriveFolder
            {
                Id = file.Id,
                Name = string.IsNullOrWhiteSpace(file.Name) ? "Izenik gabe" : file.Name,
                WebViewLink = file.WebViewLink ?? ""
            })
            .ToList() ?? [];
    }

    public async Task<DriveSaveResult> UploadFileAsync(
        string folderId,
        string folderName,
        string fileName,
        string contentType,
        byte[] content,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folderId))
        {
            throw new InvalidOperationException("Aukeratu Drive karpeta bat.");
        }

        string token = await googleAuthService.GetRequiredAccessTokenAsync(DriveScopes, cancellationToken);
        object metadata = new
        {
            name = fileName,
            parents = new[] { folderId }
        };

        using MultipartContent multipart = new("related");
        multipart.Add(new StringContent(JsonSerializer.Serialize(metadata, JsonOptions), Encoding.UTF8, "application/json"));
        ByteArrayContent fileContent = new(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        multipart.Add(fileContent);

        using HttpRequestMessage request = new(HttpMethod.Post, "https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart&fields=id,name,webViewLink&supportsAllDrives=true");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = multipart;

        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(DriveError(json, "Fitxategia ezin izan da Drive-n gorde."));
        }

        DriveFile? file = JsonSerializer.Deserialize<DriveFile>(json, JsonOptions);
        return new DriveSaveResult
        {
            FileName = file?.Name ?? fileName,
            FolderName = folderName,
            WebViewLink = file?.WebViewLink ?? ""
        };
    }

    private static string EscapeDriveQuery(string value) => value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("'", "\\'", StringComparison.Ordinal);

    private static string DriveError(string json, string fallback)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            if (root.TryGetProperty("error", out JsonElement error)
                && error.TryGetProperty("message", out JsonElement message))
            {
                return message.GetString() ?? fallback;
            }
        }
        catch (JsonException)
        {
        }

        return fallback;
    }

    private sealed class DriveFileList
    {
        public IReadOnlyList<DriveFile> Files { get; set; } = [];
    }

    private sealed class DriveFile
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? WebViewLink { get; set; }
    }
}
