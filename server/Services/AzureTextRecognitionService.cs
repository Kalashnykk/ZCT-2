using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Linq;

namespace server.Services
{
    public class AzureTextRecognitionService
    {
        private readonly string _visionKey;
        private readonly string _visionEndpoint;
        private readonly ComputerVisionClient _client;

        public AzureTextRecognitionService()
        {
            _visionKey = Environment.GetEnvironmentVariable("VISION_KEY")
                         ?? throw new InvalidOperationException("VISION_KEY not set in environment variables.");

            _visionEndpoint = Environment.GetEnvironmentVariable("VISION_ENDPOINT")
                              ?? throw new InvalidOperationException("VISION_ENDPOINT not set in environment variables.");

            _client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_visionKey))
            {
                Endpoint = _visionEndpoint
            };
        }

        public async Task<string> ReadTextFromImageAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Image file not found.", imagePath);

            using var stream = File.OpenRead(imagePath);

            var textHeaders = await _client.ReadInStreamAsync(stream);
            string operationLocation = textHeaders.OperationLocation;

            if (string.IsNullOrEmpty(operationLocation))
                throw new Exception("Failed to get operation location from Azure Read API.");

            string operationId = operationLocation.Split('/').Last();

            ReadOperationResult results;
            do
            {
                await Task.Delay(1000);
                results = await _client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);

            if (results.Status != OperationStatusCodes.Succeeded)
                throw new Exception("Text extraction failed.");

            var textOutput = results.AnalyzeResult.ReadResults
                .SelectMany(p => p.Lines)
                .Select(line => line.Text)
                .ToList();

            return string.Join(Environment.NewLine, textOutput);
        }
    }
}
