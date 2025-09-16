using CustomerImageApp.Data;
using CustomerImageApp.DTOs;
using CustomerImageApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApp.Services
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _context;
        private const int MAX_IMAGES_PER_CUSTOMER = 10;
        private const long MAX_FILE_SIZE_BYTES = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

        public ImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CustomerImageResponse>>> UploadImagesAsync(UploadImageRequest request)
        {
            try
            {

                var customer = await _context.Customers
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == request.CustomerId);

                if (customer == null)
                {
                    return ApiResponse<List<CustomerImageResponse>>.ErrorResponse("Customer not found.");
                }


                var currentImageCount = customer.Images.Count;
                var totalImagesAfterUpload = currentImageCount + request.Images.Count;

                if (totalImagesAfterUpload > MAX_IMAGES_PER_CUSTOMER)
                {
                    var remainingSlots = MAX_IMAGES_PER_CUSTOMER - currentImageCount;
                    return ApiResponse<List<CustomerImageResponse>>.ErrorResponse(
                        $"Cannot upload {request.Images.Count} images. Customer already has {currentImageCount} images. " +
                        $"Maximum allowed is {MAX_IMAGES_PER_CUSTOMER}. You can upload {remainingSlots} more image(s).");
                }


                var validationErrors = new List<string>();
                var validatedImages = new List<(ImageUploadDto dto, ImageValidationResult validation)>();

                foreach (var imageDto in request.Images)
                {
                    var validation = ValidateImage(imageDto);
                    validatedImages.Add((imageDto, validation));

                    if (!validation.IsValid)
                    {
                        validationErrors.Add($"File '{imageDto.FileName}': {validation.ErrorMessage}");
                    }
                }

                if (validationErrors.Any())
                {
                    return ApiResponse<List<CustomerImageResponse>>.ErrorResponse(
                        "Image validation failed.", validationErrors);
                }


                var customerImages = new List<CustomerImage>();
                foreach (var (dto, validation) in validatedImages)
                {
                    var customerImage = new CustomerImage
                    {
                        CustomerId = request.CustomerId,
                        ImageData = dto.ImageData,
                        FileName = dto.FileName,
                        ContentType = dto.ContentType,
                        Description = dto.Description,
                        FileSizeBytes = validation.FileSizeBytes,
                        UploadedAt = DateTime.UtcNow
                    };
                    customerImages.Add(customerImage);
                }

                _context.CustomerImages.AddRange(customerImages);
                customer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();


                var response = customerImages.Select(MapToResponse).ToList();
                return ApiResponse<List<CustomerImageResponse>>.SuccessResponse(
                    response, $"Successfully uploaded {customerImages.Count} image(s).");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CustomerImageResponse>>.ErrorResponse(
                    "An error occurred while uploading images.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<CustomerImageResponse>>> GetCustomerImagesAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    return ApiResponse<List<CustomerImageResponse>>.ErrorResponse("Customer not found.");
                }

                var images = await _context.CustomerImages
                    .Where(i => i.CustomerId == customerId)
                    .OrderBy(i => i.UploadedAt)
                    .ToListAsync();

                var response = images.Select(MapToResponse).ToList();
                return ApiResponse<List<CustomerImageResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CustomerImageResponse>>.ErrorResponse(
                    "An error occurred while retrieving images.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteImageAsync(int imageId, int customerId)
        {
            try
            {
                var image = await _context.CustomerImages
                    .FirstOrDefaultAsync(i => i.Id == imageId && i.CustomerId == customerId);

                if (image == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Image not found or doesn't belong to this customer.");
                }

                _context.CustomerImages.Remove(image);


                var customer = await _context.Customers.FindAsync(customerId);
                if (customer != null)
                {
                    customer.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Image deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the image.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CustomerImageResponse>> GetImageAsync(int imageId)
        {
            try
            {
                var image = await _context.CustomerImages.FindAsync(imageId);
                if (image == null)
                {
                    return ApiResponse<CustomerImageResponse>.ErrorResponse("Image not found.");
                }

                var response = MapToResponse(image);
                return ApiResponse<CustomerImageResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerImageResponse>.ErrorResponse(
                    "An error occurred while retrieving the image.", new List<string> { ex.Message });
            }
        }

        private ImageValidationResult ValidateImage(ImageUploadDto imageDto)
        {
            try
            {

                if (!_allowedContentTypes.Contains(imageDto.ContentType.ToLower()))
                {
                    return new ImageValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Invalid content type '{imageDto.ContentType}'. Allowed types: {string.Join(", ", _allowedContentTypes)}"
                    };
                }


                if (string.IsNullOrWhiteSpace(imageDto.ImageData))
                {
                    return new ImageValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Image data is required."
                    };
                }


                string base64Data = imageDto.ImageData;
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                byte[] imageBytes;
                try
                {
                    imageBytes = Convert.FromBase64String(base64Data);
                }
                catch (FormatException)
                {
                    return new ImageValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid base64 format."
                    };
                }


                if (imageBytes.Length > MAX_FILE_SIZE_BYTES)
                {
                    return new ImageValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"File size ({imageBytes.Length:N0} bytes) exceeds maximum allowed size ({MAX_FILE_SIZE_BYTES:N0} bytes)."
                    };
                }


                if (imageBytes.Length < 100)
                {
                    return new ImageValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "File appears to be too small to be a valid image."
                    };
                }

                return new ImageValidationResult
                {
                    IsValid = true,
                    FileSizeBytes = imageBytes.Length
                };
            }
            catch (Exception ex)
            {
                return new ImageValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Validation error: {ex.Message}"
                };
            }
        }

        private static CustomerImageResponse MapToResponse(CustomerImage image)
        {
            return new CustomerImageResponse
            {
                Id = image.Id,
                CustomerId = image.CustomerId,
                ImageData = image.ImageData,
                FileName = image.FileName,
                ContentType = image.ContentType,
                FileSizeBytes = image.FileSizeBytes,
                UploadedAt = image.UploadedAt,
                Description = image.Description
            };
        }
    }
}