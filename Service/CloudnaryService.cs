using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using WebApplication1.Interface;

namespace WebApplication1.Service;

public class CloudnaryService : ICloudnaryInterface
{
    private readonly Cloudinary _cloudinary;

    public CloudnaryService()
    {
        try
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

        }
        catch (Exception ex)
        {

            throw new InvalidOperationException("failed to load env file", ex);
        }

        var cloudnaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");

        if (string.IsNullOrEmpty(cloudnaryUrl))
        {
            throw new InvalidOperationException("CLOUDINARY_URL environment variable is not set.");
        }
        _cloudinary = new Cloudinary(cloudnaryUrl) { Api = { Secure = true } };
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is invalid.");
        }

        using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
            Folder = "Fuzzy Goggles"
            // Transformation = new Transformation().Width(150).Height(150).Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }
}