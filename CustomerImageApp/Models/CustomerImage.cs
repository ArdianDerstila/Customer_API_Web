using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerImageApp.Models
{
    public class CustomerImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string ImageData { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSizeBytes { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;


        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }
}