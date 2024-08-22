using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime EntryDate { get; set; }

        [Required]
        [StringLength(100)]
        public string?  Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? Description { get; set; }

        public bool IsInvoiced { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
