using FastVocab.Application.Common.Models;
using FastVocab.Shared.Utils;
using Microsoft.AspNetCore.Http;

namespace FastVocab.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<Result<FileMetadata>> UploadAsync(IFormFile file, string resource, string resourceId);
    Task<Result> DeleteFileAsync(string url);
    Task DeleteFilesAsync(string resource, string entityId);

    Task<IEnumerable<FileMetadata>> GetFileMetadatasByResourceAsync(string resource);
}

