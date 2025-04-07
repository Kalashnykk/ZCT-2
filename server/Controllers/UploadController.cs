using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly string _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadRequest request)
        {
            var image = request.Image;

            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            var ext = Path.GetExtension(image.FileName);
            var fileName = $"image_{timestamp}{ext}";
            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { message = "Image uploaded successfully", fileName });
        }
    }
}
