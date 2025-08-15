using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class WorkOrder
    {
        [Key]
        public int WorkOrderId { get; set; }

        [Required]
        [StringLength(20)]
        public string WorkOrderNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int WorkOrderTypeId { get; set; }

        public int CategoryId { get; set; }

        public int StatusId { get; set; }

        public int? TechnicianId { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public TimeSpan? ScheduledStartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? EstimatedHours { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ActualHours { get; set; }

        public DateTime? CompletedDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? EstimatedCost { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ActualCost { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("WorkOrderTypeId")]
        public virtual WorkOrderType WorkOrderType { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual WorkOrderCategory Category { get; set; } = null!;

        [ForeignKey("StatusId")]
        public virtual WorkOrderStatus Status { get; set; } = null!;

        [ForeignKey("TechnicianId")]
        public virtual Technician? Technician { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<WorkOrderItem> WorkOrderItems { get; set; } = new List<WorkOrderItem>();
    }
}