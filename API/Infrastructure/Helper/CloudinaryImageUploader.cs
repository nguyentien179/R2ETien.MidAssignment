using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using mid_assignment.Application.Interfaces;
using mid_assignment.Infrastructure.EntityConfig;

namespace mid_assignment.Infrastructure.Helper;

public class CloudinaryImageUploader : IImageUploader
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageUploader(IOptions<CloudinarySettings> options)
    {
        var settings = options.Value;
        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        if (file.Length <= 0)
            return null;

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult.StatusCode == System.Net.HttpStatusCode.OK
            ? uploadResult.SecureUrl.AbsoluteUri
            : null;
    }
}
