using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class UploadRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;

        public bool Solve { get; set; } = false;
    }
}