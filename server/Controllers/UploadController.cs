using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using server.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AzureTextRecognitionService _textRecognitionService;
        private readonly MyDbContext _dbContext;

        public UploadController(AzureTextRecognitionService textRecognitionService, MyDbContext dbContext)
        {
            _textRecognitionService = textRecognitionService;
            _dbContext = dbContext;
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
            var imageUrl = $"{Request.Scheme}://{Request.Host}/Images/{fileName}";

            int? result = null;

            //If user chosed to solve the math expression
            if (request.Solve)
            {
                var validator = new MathExpressionValidator();
                
                // Validate the expression
                if (validator.Validate(extractedText, out var errors, out string valid_string))
                {
                    // Calculate the result if string is valid
                    result = Calculator.ToCount(valid_string);
                }
            }

            // ✅ Save to database
            var historyRecord = new tUploadHistory
            {
                Image_url = imageUrl,
                Extracted_text = extractedText,
                Result = result ?? 0,  // Use 0 if no result calculated
                DataTime = DateTime.UtcNow
            };

            _dbContext.tUploadHistory.Add(historyRecord);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Image uploaded and processed successfully",
                fileName,
                imageUrl,
                extractedText,
                result
            });
        }
    }
}
