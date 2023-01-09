using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FlatRenting.Interfaces;
using System.Net;

namespace FlatRenting.Services;

public class PhotoService : IPhotoService {
    private readonly IConfiguration _config;

    public PhotoService(IConfiguration config) => _config = config;

    public async Task<string> UploadPhoto(IFormFile imgFile) {
        var cloudinary = new Cloudinary(_config["Cloudinary:Url"]);
        cloudinary.Api.Secure = true;

        using var stream = imgFile.OpenReadStream();

        var uploadParams = new ImageUploadParams() {
            File = new FileDescription(imgFile.FileName, stream),
            UniqueFilename = true,
            Overwrite = false,
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        return uploadResult?.SecureUri.AbsoluteUri;
    }
}
