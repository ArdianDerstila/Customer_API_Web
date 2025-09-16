
using System.ComponentModel.DataAnnotations;

public class CreateImageRequest
{
    [Required]
    public string ImageData { get; set; } = string.Empty;

    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string ContentType { get; set; } = string.Empty;

    public string? Description { get; set; }
}