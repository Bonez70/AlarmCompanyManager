using System.ComponentModel.DataAnnotations;

namespace AlarmCompanyManager.Models
{
    public class WorkOrderStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(7)]
        public string? ColorCode { get; set; } // For UI color coding

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}