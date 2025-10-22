using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FastVocab.Application.Common.Interfaces;
using FastVocab.Application.Common.Models;
using FastVocab.Infrastructure.Data.EFCore;
using FastVocab.Infrastructure.Extensions.Options;
using FastVocab.Shared.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Error = FastVocab.Shared.Utils.Error;

namespace FastVocab.Infrastructure.Services.FileServices;

public class CloudinaryService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly AppDbContext _context;

    public CloudinaryService(IOptions<CloudinarySettings> config, AppDbContext context)
    {
        var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
        _cloudinary = new Cloudinary(acc);
        _context = context;
    }

    public async Task<IEnumerable<FileMetadata>> GetFileMetadatasByResourceAsync(string resource)
    {
        return await _context.FileMetadata.Where(f => f.Resource == resource).ToListAsync();
    }

    public async Task<Result<FileMetadata>> UploadAsync(IFormFile file, string resource, string entityId)
    {
        // validate
        if (file.Length == 0)
        {
            return Result<FileMetadata>.Failure(Error.InvalidInput);
        }

        var uploadResult = await UploadToCloudAsync(file, $"FastVocab_{resource}");

        if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            return Result<FileMetadata>.Failure(Error.ExternalServiceError);

        // Save
        var metadata = new FileMetadata
        {
            Url = uploadResult.SecureUrl.AbsoluteUri,
            Resource = resource,
            EntityId = entityId,
            Format = uploadResult.Format ?? string.Empty,
            FileName = uploadResult.OriginalFilename ?? string.Empty,
            ExternalId = uploadResult.PublicId,
            Type = uploadResult.Type ?? string.Empty,
            Folder = uploadResult.AssetFolder ?? string.Empty,
        };

        _context.FileMetadata.Add(metadata);
        await _context.SaveChangesAsync();

        return Result<FileMetadata>.Success(metadata);
    }

    public async Task DeleteFilesAsync(string resource, string entityId)
    {
        var files = await _context.FileMetadata.Where(f => f.Resource == resource && f.EntityId == entityId).ToListAsync();

        foreach (var metadata in files)
        {
            await DeleteFromCloudAsync(metadata.ExternalId);
        }

        _context.FileMetadata.RemoveRange(files);
        await _context.SaveChangesAsync();
    }

    public async Task<Result> DeleteFileAsync(string url)
    {
        var metadata = await _context.FileMetadata.FirstOrDefaultAsync(f => f.Url == url);

        if (metadata == null)
        {
            return Result.Failure(Error.NotFound);
        }

        var result = await DeleteFromCloudAsync(metadata.ExternalId);

        if (!result)
        {
            return Result.Failure(Error.ExternalServiceError);
        }

        _context.FileMetadata.Remove(metadata);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<ImageUploadResult> UploadToCloudAsync(IFormFile file, string folder)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }

    private async Task<bool> DeleteFromCloudAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result == "ok";
    }
}