using CustomerImageApp.Data;
using CustomerImageApp.DTOs;
using CustomerImageApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerImageApp.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _context.Customers
                    .Include(c => c.Images)
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();

                var response = customers.Select(MapToResponse).ToList();
                return ApiResponse<List<CustomerResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CustomerResponse>>.ErrorResponse(
                    "An error occurred while retrieving customers.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                {
                    return ApiResponse<CustomerResponse>.ErrorResponse("Customer not found.");
                }

                var response = MapToResponse(customer);
                return ApiResponse<CustomerResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerResponse>.ErrorResponse(
                    "An error occurred while retrieving the customer.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {

                var customer = new Customer
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Company = request.Company,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Images = new List<CustomerImage>()
                };


                if (request.Images != null && request.Images.Any())
                {
                    foreach (var imageRequest in request.Images)
                    {

                        if (string.IsNullOrWhiteSpace(imageRequest.ImageData))
                            continue;

                        try
                        {
                            var imageBytes = Convert.FromBase64String(imageRequest.ImageData);

                            var image = new CustomerImage
                            {
                                ImageData = imageRequest.ImageData,
                                FileName = imageRequest.FileName,
                                ContentType = imageRequest.ContentType,
                                FileSizeBytes = imageBytes.Length,
                                UploadedAt = DateTime.UtcNow,
                                Description = imageRequest.Description,
                                Customer = customer
                            };

                            customer.Images.Add(image);
                        }
                        catch (FormatException)
                        {

                            throw new ArgumentException($"Invalid Base64 string in image: {imageRequest.FileName}");
                        }
                    }
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var response = MapToResponse(customer);
                return ApiResponse<CustomerResponse>.SuccessResponse(response, "Customer created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerResponse>.ErrorResponse(
                    "An error occurred while creating the customer.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(int id, UpdateCustomerRequest request)
        {
            try
            {
                var existingCustomer = await _context.Customers
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existingCustomer == null)
                {
                    return ApiResponse<CustomerResponse>.ErrorResponse("Customer not found.");
                }


                existingCustomer.FirstName = request.FirstName;
                existingCustomer.LastName = request.LastName;
                existingCustomer.Email = request.Email;
                existingCustomer.Phone = request.Phone;
                existingCustomer.Company = request.Company;
                existingCustomer.UpdatedAt = DateTime.UtcNow;


                if (request.Images != null && request.Images.Any())
                {

                    _context.CustomerImages.RemoveRange(existingCustomer.Images);


                    foreach (var imageRequest in request.Images)
                    {
                        var imageBytes = Convert.FromBase64String(imageRequest.ImageData);

                        var image = new CustomerImage
                        {
                            CustomerId = id,
                            ImageData = imageRequest.ImageData,
                            FileName = imageRequest.FileName,
                            ContentType = imageRequest.ContentType,
                            FileSizeBytes = imageBytes.Length,
                            UploadedAt = DateTime.UtcNow,
                            Description = imageRequest.Description
                        };

                        existingCustomer.Images.Add(image);
                    }
                }

                await _context.SaveChangesAsync();

                var response = MapToResponse(existingCustomer);
                return ApiResponse<CustomerResponse>.SuccessResponse(response, "Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerResponse>.ErrorResponse(
                    "An error occurred while updating the customer.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer not found.");
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Customer deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the customer.", new List<string> { ex.Message });
            }
        }

        private static CustomerResponse MapToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Company = customer.Company,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                ImageCount = customer.ImageCount,
                CanAddMoreImages = customer.CanAddMoreImages,
                Images = customer.Images?.Select(img => new CustomerImageResponse
                {
                    Id = img.Id,
                    CustomerId = img.CustomerId,
                    ImageData = img.ImageData,
                    FileName = img.FileName,
                    ContentType = img.ContentType,
                    FileSizeBytes = img.FileSizeBytes,
                    UploadedAt = img.UploadedAt,
                    Description = img.Description
                }).OrderBy(img => img.UploadedAt).ToList() ?? new List<CustomerImageResponse>()
            };
        }
    }
}