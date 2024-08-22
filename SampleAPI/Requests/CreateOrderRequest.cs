using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Entities
{
    public class CreateOrderRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name length cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Description length cannot exceed 100 characters.")]
        public string? Description { get; set; }
    }
}
