using CustomerImageApp.DTOs;
using CustomerImageApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerImageApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CustomerResponse>>>> GetAllCustomers()
        {
            var result = await _customerService.GetAllCustomersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetCustomerById(int id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<CustomerResponse>.ErrorResponse("Validation failed.", errors));
            }

            var result = await _customerService.CreateCustomerAsync(request);
            return result.Success
                ? CreatedAtAction(nameof(GetCustomerById), new { id = result.Data!.Id }, result)
                : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<CustomerResponse>.ErrorResponse("Validation failed.", errors));
            }

            var result = await _customerService.UpdateCustomerAsync(id, request);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}