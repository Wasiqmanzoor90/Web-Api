using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Interface
{
    public interface ICloudnaryInterface
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}