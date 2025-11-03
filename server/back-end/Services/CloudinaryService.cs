using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Cryptography;
using System.Text;

namespace back_end.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly string _cloudName;
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
        {
            _logger = logger;

            _cloudName = configuration["Cloudinary:CloudName"] 
                ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured");
            _apiKey = configuration["Cloudinary:ApiKey"] 
                ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured");
            _apiSecret = configuration["Cloudinary:ApiSecret"] 
                ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured");
        }

        public CloudinaryUploadParams GenerateUploadSignature(string folder = "events")
        {
            // Generate timestamp (seconds since epoch)
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Build parameters to sign (alphabetically ordered, excluding signature and api_key)
            var paramsToSign = new SortedDictionary<string, object>
            {
                { "folder", folder },
                { "timestamp", timestamp },
                { "upload_preset", "" } // Optional: you can create upload presets in Cloudinary dashboard
            };

            // Remove empty values
            var filteredParams = paramsToSign
                .Where(p => p.Value != null && !string.IsNullOrEmpty(p.Value.ToString()))
                .ToDictionary(p => p.Key, p => p.Value);

            // Create signature string
            var signatureString = string.Join("&", 
                filteredParams.Select(p => $"{p.Key}={p.Value}"));
            
            // Append API secret
            signatureString += _apiSecret;

            // Generate SHA256 signature
            var signature = ComputeSha256Hash(signatureString);

            _logger.LogInformation("Generated Cloudinary upload signature for folder: {Folder}", folder);

            return new CloudinaryUploadParams
            {
                ApiKey = _apiKey,
                CloudName = _cloudName,
                Signature = signature,
                Timestamp = timestamp,
                Folder = folder,
                UploadUrl = $"https://api.cloudinary.com/v1_1/{_cloudName}/image/upload"
            };
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
