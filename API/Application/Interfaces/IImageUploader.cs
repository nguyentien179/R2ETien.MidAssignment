using System;

namespace mid_assignment.Application.Interfaces;

public interface IImageUploader
{
    Task<string?> UploadImageAsync(IFormFile file);
}
