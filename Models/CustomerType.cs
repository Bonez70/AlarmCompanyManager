using System.ComponentModel.DataAnnotations;

namespace AlarmCompanyManager.Models
{
    public class CustomerType
    {
        [Key]
        public int CustomerTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}