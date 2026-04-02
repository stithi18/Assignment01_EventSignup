using Microsoft.AspNetCore.Http;

namespace Assignment01_EventSignup.Services
{
    public interface IBlobService
    {
        Task<string?> UploadFileAsync(IFormFile? file);
    }
}