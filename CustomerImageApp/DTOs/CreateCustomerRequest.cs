
using System.ComponentModel.DataAnnotations;

public class CreateCustomerRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public List<CreateImageRequest>? Images { get; set; }
}

