using System.ComponentModel.DataAnnotations;

namespace AlarmCompanyManager.Models
{
    public class PanelType
    {
        [Key]
        public int PanelTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ModelNumber { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<SecuritySystem> SecuritySystems { get; set; } = new List<SecuritySystem>();
    }
}