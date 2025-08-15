using System.ComponentModel.DataAnnotations;

namespace AlarmCompanyManager.Models
{
    public class MonitoringType
    {
        [Key]
        public int MonitoringTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<SecuritySystem> SecuritySystems { get; set; } = new List<SecuritySystem>();
    }
}