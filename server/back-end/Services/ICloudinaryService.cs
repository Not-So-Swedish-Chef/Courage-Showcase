namespace back_end.Services
{
    public interface ICloudinaryService
    {
        CloudinaryUploadParams GenerateUploadSignature(string folder = "events");
    }

    public class CloudinaryUploadParams
    {
        public string ApiKey { get; set; } = "";
        public string CloudName { get; set; } = "";
        public string Signature { get; set; } = "";
        public long Timestamp { get; set; }
        public string Folder { get; set; } = "";
        public string UploadUrl { get; set; } = "";
    }
}
