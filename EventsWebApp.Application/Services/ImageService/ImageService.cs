﻿using EventsWebApp.Application.Dto;
using EventsWebApp.Application.Interfaces;

namespace EventsWebApp.Application.Services.ImageService
{
    public class ImageService : IImageService
    {
        public async Task<string> StoreImage(StoreImageRequest request, CancellationToken cancellationToken)
        {
            if (request.Image == null)
            {
                return string.Empty;
            }
            var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(request.Image.FileName)}";
            var filePath = Path.Combine(request.Path, "images", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await request.Image.CopyToAsync(stream);
            }

            return Path.Combine("images", fileName);
        }
    }
}