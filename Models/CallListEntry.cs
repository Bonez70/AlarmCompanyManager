using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class CallListEntry
    {
        [Key]
        public int CallListEntryId { get; set; }

        public int SecuritySystemId { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("SecuritySystemId")]
        public virtual SecuritySystem SecuritySystem { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}