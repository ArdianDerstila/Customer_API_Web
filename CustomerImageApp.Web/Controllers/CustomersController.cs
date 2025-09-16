using CustomerImageApp.Web.Models;
using CustomerImageApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerImageApp.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerImageService _customerImageService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerImageService customerImageService, ILogger<CustomersController> logger)
        {
            _customerImageService = customerImageService;
            _logger = logger;
        }

        // GET: Customer/List
        public async Task<IActionResult> List()
        {
            try
            {
                var result = await _customerImageService.GetAllCustomersAsync();

                var viewModel = new CustomerListViewModel
                {
                    Customers = result.Success ? result.Data ?? new List<CustomerResponse>() : new List<CustomerResponse>(),
                    Message = !result.Success ? result.Message : null,
                    IsSuccess = result.Success
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers list");

                var viewModel = new CustomerListViewModel
                {
                    Customers = new List<CustomerResponse>(),
                    Message = "An error occurred while loading customers.",
                    IsSuccess = false
                };

                return View(viewModel);
            }
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View(new CustomerViewModel());
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new CreateCustomerRequest
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Company = model.Company,
                    Images = new List<CustomerImageUploadDto>() // Empty for now
                };

                var result = await _customerImageService.CreateCustomerAsync(request);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Customer created successfully!";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    if (result.Errors?.Any() == true)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                TempData["ErrorMessage"] = "An error occurred while creating the customer.";
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new UpdateCustomerRequest
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Company = model.Company
                };

                var result = await _customerImageService.UpdateCustomerAsync(id, request);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Customer updated successfully!";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    if (result.Errors?.Any() == true)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating the customer.";
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var result = await _customerImageService.GetCustomerByIdAsync(id);

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = "Customer not found.";
                    return RedirectToAction("List");
                }

                var viewModel = new CustomerViewModel
                {
                    Id = result.Data.Id,
                    FirstName = result.Data.FirstName,
                    LastName = result.Data.LastName,
                    Email = result.Data.Email,
                    Phone = result.Data.Phone,
                    Company = result.Data.Company,
                    CreatedAt = result.Data.CreatedAt
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the customer.";
                return RedirectToAction("List");
            }
        }



        // POST: Customer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _customerImageService.DeleteCustomerAsync(id);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Customer deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Failed to delete customer.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the customer.";
            }

            return RedirectToAction("List");
        }


    }
}