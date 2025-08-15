using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class Technician
    {
        [Key]
        public int TechnicianId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EmailAddress { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(15)]
        public string? CellPhone { get; set; }

        [StringLength(20)]
        public string? EmployeeNumber { get; set; }

        public DateTime? HireDate { get; set; }

        [StringLength(200)]
        public string? Specializations { get; set; }

        [StringLength(200)]
        public string? Certifications { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}