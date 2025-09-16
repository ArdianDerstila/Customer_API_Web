using CustomerImageApp.DTOs;
using CustomerImageApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerImageApp.Controllers
{
    [ApiController]
    [Route("api/customers/{customerId}/images")]
    public class CustomerImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public CustomerImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<List<CustomerImageResponse>>>> UploadImages(
            int customerId,
            [FromBody] List<ImageUploadDto> images)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<List<CustomerImageResponse>>.ErrorResponse("Validation failed.", errors));
            }

            var request = new UploadImageRequest
            {
                CustomerId = customerId,
                Images = images
            };

            var result = await _imageService.UploadImagesAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CustomerImageResponse>>>> GetCustomerImages(int customerId)
        {
            var result = await _imageService.GetCustomerImagesAsync(customerId);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpDelete("{imageId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteImage(int customerId, int imageId)
        {
            var result = await _imageService.DeleteImageAsync(imageId, customerId);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [HttpGet("{imageId}")]
        public async Task<ActionResult<ApiResponse<CustomerImageResponse>>> GetImage(int customerId, int imageId)
        {
            var result = await _imageService.GetImageAsync(imageId);

            if (!result.Success)
                return NotFound(result);


            if (result.Data!.CustomerId != customerId)
                return NotFound(ApiResponse<CustomerImageResponse>.ErrorResponse("Image not found for this customer."));

            return Ok(result);
        }
    }
}