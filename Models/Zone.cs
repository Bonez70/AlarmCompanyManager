using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class Zone
    {
        [Key]
        public int ZoneId { get; set; }

        public int SecuritySystemId { get; set; }

        [Required]
        public int ZoneNumber { get; set; }

        [StringLength(10)]
        public string? Signal { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        public int? DeviceTypeId { get; set; }

        [StringLength(20)]
        public string? WirelessId { get; set; }

        [ForeignKey("SecuritySystemId")]
        public virtual SecuritySystem SecuritySystem { get; set; } = null!;

        [ForeignKey("DeviceTypeId")]
        public virtual DeviceType? DeviceType { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}