using CustomerImageApp.Web.Models;
using System.Text;
using System.Text.Json;

namespace CustomerImageApp.Web.Services
{
    public class CustomerImageService : ICustomerImageService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CustomerImageService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public CustomerImageService(IHttpClientFactory httpClientFactory, ILogger<CustomerImageService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // Image Methods
        public async Task<ApiResponse<List<CustomerImageResponse>>> GetCustomerImagesAsync(int customerId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var response = await client.GetAsync($"api/customers/{customerId}/images");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<List<CustomerImageResponse>>>(content, _jsonOptions);
                    return result ?? CreateErrorResponse<List<CustomerImageResponse>>("Failed to parse response");
                }
                else
                {
                    return CreateErrorResponse<List<CustomerImageResponse>>($"API returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer images for customer {CustomerId}", customerId);
                return CreateErrorResponse<List<CustomerImageResponse>>("Error connecting to API");
            }
        }

        public async Task<ApiResponse<List<CustomerImageResponse>>> UploadImagesAsync(int customerId, List<ImageUploadDto> images)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var json = JsonSerializer.Serialize(images, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"api/customers/{customerId}/images", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<List<CustomerImageResponse>>>(responseContent, _jsonOptions);
                    return result ?? CreateErrorResponse<List<CustomerImageResponse>>("Failed to parse response");
                }
                else
                {
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<List<CustomerImageResponse>>>(responseContent, _jsonOptions);
                    return errorResult ?? CreateErrorResponse<List<CustomerImageResponse>>($"Upload failed with status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading images for customer {CustomerId}", customerId);
                return CreateErrorResponse<List<CustomerImageResponse>>("Error uploading to API");
            }
        }

        public async Task<ApiResponse<bool>> DeleteImageAsync(int customerId, int imageId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var response = await client.DeleteAsync($"api/customers/{customerId}/images/{imageId}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                    return result ?? CreateErrorResponse<bool>("Failed to parse response");
                }
                else
                {
                    return CreateErrorResponse<bool>($"Delete failed with status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId} for customer {CustomerId}", imageId, customerId);
                return CreateErrorResponse<bool>("Error deleting from API");
            }
        }

        public async Task<ApiResponse<CustomerImageResponse>> GetImageAsync(int imageId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var response = await client.GetAsync($"api/customers/1/images/{imageId}"); // Note: You might need to adjust this endpoint

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<CustomerImageResponse>>(content, _jsonOptions);
                    return result ?? CreateErrorResponse<CustomerImageResponse>("Failed to parse response");
                }
                else
                {
                    return CreateErrorResponse<CustomerImageResponse>($"API returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image {ImageId}", imageId);
                return CreateErrorResponse<CustomerImageResponse>("Error connecting to API");
            }
        }

        // Customer Methods
        public async Task<ApiResponse<List<CustomerResponse>>> GetAllCustomersAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var response = await client.GetAsync("api/customers");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<List<CustomerResponse>>>(content, _jsonOptions);
                    return result ?? CreateErrorResponse<List<CustomerResponse>>("Failed to parse response");
                }
                else
                {
                    return CreateErrorResponse<List<CustomerResponse>>($"API returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                return CreateErrorResponse<List<CustomerResponse>>("Error connecting to API");
            }
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var response = await client.GetAsync($"api/customers/{id}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<CustomerResponse>>(content, _jsonOptions);
                    return result ?? CreateErrorResponse<CustomerResponse>("Failed to parse response");
                }
                else
                {
                    return CreateErrorResponse<CustomerResponse>($"Customer not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {CustomerId}", id);
                return CreateErrorResponse<CustomerResponse>("Error connecting to API");
            }
        }

        public async Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/customers", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<CustomerResponse>>(responseContent, _jsonOptions);
                    return result ?? CreateErrorResponse<CustomerResponse>("Failed to parse response");
                }
                else
                {
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<CustomerResponse>>(responseContent, _jsonOptions);
                    return errorResult ?? CreateErrorResponse<CustomerResponse>($"Create failed with status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return CreateErrorResponse<CustomerResponse>("Error creating customer");
            }
        }

        public async Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(int id, UpdateCustomerRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/customers/{id}", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<CustomerResponse>>(responseContent, _jsonOptions);
                    return result ?? CreateErrorResponse<CustomerResponse>("Failed to parse response");
                }
                else
                {
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<CustomerResponse>>(responseContent, _jsonOptions);
                    return errorResult ?? CreateErrorResponse<CustomerResponse>($"Update failed with status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", id);
                return CreateErrorResponse<CustomerResponse>("Error updating customer");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CustomerAPI");
                var response = await client.DeleteAsync($"api/customers/{id}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ApiResponse<bool>>(content, _jsonOptions);
                    return result ?? CreateErrorResponse<bool>("Failed to parse response");
                }
                else
                {
                    return CreateErrorResponse<bool>($"Delete failed with status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                return CreateErrorResponse<bool>("Error deleting customer");
            }
        }

        // Helper method to create error responses
        private static ApiResponse<T> CreateErrorResponse<T>(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }
}