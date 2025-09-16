using CustomerImageApp.Web.Models;

namespace CustomerImageApp.Web.Services
{
    public interface ICustomerImageService
    {
        // Image management
        Task<ApiResponse<List<CustomerImageResponse>>> GetCustomerImagesAsync(int customerId);
        Task<ApiResponse<List<CustomerImageResponse>>> UploadImagesAsync(int customerId, List<ImageUploadDto> images);
        Task<ApiResponse<bool>> DeleteImageAsync(int customerId, int imageId);
        Task<ApiResponse<CustomerImageResponse>> GetImageAsync(int imageId);

        // Customer management
        Task<ApiResponse<List<CustomerResponse>>> GetAllCustomersAsync();
        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id);
        Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request);
        Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(int id, UpdateCustomerRequest request);
        Task<ApiResponse<bool>> DeleteCustomerAsync(int id);
    }
}