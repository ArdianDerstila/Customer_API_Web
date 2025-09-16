namespace CustomerImageApp.Web.Models
{
    // Customer Image Response from API
    public class CustomerImageResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string ImageData { get; set; } = string.Empty;
    }

    // Image Upload DTO (for separate image upload)
    public class ImageUploadDto
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string ImageData { get; set; } = string.Empty;
    }

    // Customer Image Upload DTO (for creating customer with images)
    public class CustomerImageUploadDto
    {
        public string ImageData { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}