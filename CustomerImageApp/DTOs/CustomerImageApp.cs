using System.ComponentModel.DataAnnotations;

namespace CustomerImageApp.DTOs
{

    public class UploadImageRequest
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<ImageUploadDto> Images { get; set; } = new();
    }

    public class ImageUploadDto
    {
        [Required]
        public string ImageData { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ContentType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
    }


    public class CustomerImageResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ImageData { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class CustomerResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ImageCount { get; set; }
        public bool CanAddMoreImages { get; set; }
        public List<CustomerImageResponse> Images { get; set; } = new();
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class ImageValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
    }
}