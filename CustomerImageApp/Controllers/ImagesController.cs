using CustomerImageApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerImageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }


        [HttpGet("{imageId}/data")]
        public async Task<IActionResult> GetImageData(int imageId)
        {
            var result = await _imageService.GetImageAsync(imageId);

            if (!result.Success || result.Data == null)
                return NotFound();

            try
            {
                string base64Data = result.Data.ImageData;
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                var imageBytes = Convert.FromBase64String(base64Data);
                return File(imageBytes, result.Data.ContentType, result.Data.FileName);
            }
            catch (Exception)
            {
                return BadRequest("Invalid image data");
            }
        }
    }
}