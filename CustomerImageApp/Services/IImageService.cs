using CustomerImageApp.DTOs;

namespace CustomerImageApp.Services
{
    public interface IImageService
    {
        Task<ApiResponse<List<CustomerImageResponse>>> UploadImagesAsync(UploadImageRequest request);
        Task<ApiResponse<List<CustomerImageResponse>>> GetCustomerImagesAsync(int customerId);
        Task<ApiResponse<bool>> DeleteImageAsync(int imageId, int customerId);
        Task<ApiResponse<CustomerImageResponse>> GetImageAsync(int imageId);
    }
}