using System.ComponentModel.DataAnnotations;

namespace CustomerImageApp.Web.Models
{
    // View model for customer management
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Company")]
        public string Company { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Helper property for display
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    // View model for customer images page
    public class CustomerImagesViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<CustomerImageResponse> Images { get; set; } = new List<CustomerImageResponse>();
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public int ImageCount => Images.Count;
        public int MaxImages => 10;
        public bool CanUploadMore => ImageCount < MaxImages;
    }

    // View model for file uploads
    public class ImageUploadViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }

    // View model for customer list
    public class CustomerListViewModel
    {
        public List<CustomerResponse> Customers { get; set; } = new List<CustomerResponse>();
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}