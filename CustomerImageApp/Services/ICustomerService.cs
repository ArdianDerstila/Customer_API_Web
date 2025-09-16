using CustomerImageApp.DTOs;

namespace CustomerImageApp.Services
{
    public interface ICustomerService
    {
        Task<ApiResponse<List<CustomerResponse>>> GetAllCustomersAsync();
        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id);
        Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request);
        Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(int id, UpdateCustomerRequest request);
        Task<ApiResponse<bool>> DeleteCustomerAsync(int id);
    }
}