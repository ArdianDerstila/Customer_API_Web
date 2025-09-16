using CustomerImageApp.Web.Models;
using CustomerImageApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerImageApp.Web.Controllers
{
    public class CustomerImagesController : Controller
    {
        private readonly ICustomerImageService _customerImageService;
        private readonly ILogger<CustomerImagesController> _logger;

        public CustomerImagesController(ICustomerImageService customerImageService, ILogger<CustomerImagesController> logger)
        {
            _customerImageService = customerImageService;
            _logger = logger;
        }

        public async Task<IActionResult> Images(int id = 1)
        {
            try
            {

                var customerResult = await _customerImageService.GetCustomerByIdAsync(id);


                var imagesResult = await _customerImageService.GetCustomerImagesAsync(id);

                var customerName = "Unknown Customer";
                if (customerResult.Success && customerResult.Data != null)
                {
                    customerName = $"{customerResult.Data.FirstName} {customerResult.Data.LastName}".Trim();
                }

                var viewModel = new CustomerImagesViewModel
                {
                    CustomerId = id,
                    CustomerName = customerName,
                    Images = imagesResult.Success ? imagesResult.Data ?? new List<CustomerImageResponse>() : new List<CustomerImageResponse>(),
                    Message = !imagesResult.Success ? imagesResult.Message : null,
                    IsSuccess = imagesResult.Success
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer images for customer {CustomerId}", id);

                var viewModel = new CustomerImagesViewModel
                {
                    CustomerId = id,
                    CustomerName = "Error Loading Customer",
                    Images = new List<CustomerImageResponse>(),
                    Message = "An error occurred while loading customer images.",
                    IsSuccess = false
                };

                return View(viewModel);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int customerId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                TempData["ErrorMessage"] = "Please select at least one image file.";
                return RedirectToAction("Images", new { id = customerId });
            }


            var validFiles = new List<IFormFile>();
            foreach (var file in files)
            {
                if (file.Length > 0 && IsValidImageFile(file))
                {

                    if (file.Length > 5 * 1024 * 1024)
                    {
                        TempData["ErrorMessage"] = $"File {file.FileName} is too large. Maximum size is 5MB.";
                        return RedirectToAction("Images", new { id = customerId });
                    }
                    validFiles.Add(file);
                }
            }

            if (validFiles.Count == 0)
            {
                TempData["ErrorMessage"] = "No valid image files selected. Please select JPG, PNG, or GIF files.";
                return RedirectToAction("Images", new { id = customerId });
            }

            try
            {

                var imageData = new List<ImageUploadDto>();

                foreach (var file in validFiles)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        var base64String = Convert.ToBase64String(fileBytes);

                        imageData.Add(new ImageUploadDto
                        {
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            ImageData = $"data:{file.ContentType};base64,{base64String}"
                        });
                    }
                }


                var result = await _customerImageService.UploadImagesAsync(customerId, imageData);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Successfully uploaded {validFiles.Count} image(s)!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Failed to upload images.";
                    if (result.Errors?.Any() == true)
                    {
                        TempData["ErrorDetails"] = string.Join(", ", result.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading images for customer {CustomerId}", customerId);
                TempData["ErrorMessage"] = "An error occurred while uploading images.";
            }

            return RedirectToAction("Images", new { id = customerId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int customerId, int imageId, string fileName)
        {
            try
            {
                var result = await _customerImageService.DeleteImageAsync(customerId, imageId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Image '{fileName}' deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Failed to delete image.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId} for customer {CustomerId}", imageId, customerId);
                TempData["ErrorMessage"] = "An error occurred while deleting the image.";
            }

            return RedirectToAction("Images", new { id = customerId });
        }




        private static bool IsValidImageFile(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }
    }
}