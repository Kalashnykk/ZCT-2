using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AzureTextRecognitionService _textRecognitionService;
        private readonly string _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        public UploadController(AzureTextRecognitionService textRecognitionService)
        {
            _textRecognitionService = textRecognitionService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadRequest request)
        {
            var image = request.File;

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

            string extractedText = await _textRecognitionService.ReadTextFromImageAsync(path);

            return Ok(new { message = "Image uploaded and processed successfully", fileName, extractedText });
        }
    }
}
