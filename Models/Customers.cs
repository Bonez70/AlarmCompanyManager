using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [StringLength(50)]
        public string? County { get; set; }

        [StringLength(100)]
        public string? EmailAddress { get; set; }

        [StringLength(15)]
        public string? HomePhone { get; set; }

        [StringLength(15)]
        public string? BusinessPhone { get; set; }

        [StringLength(15)]
        public string? CellPhone { get; set; }

        public int CustomerTypeId { get; set; }

        [ForeignKey("CustomerTypeId")]
        public virtual CustomerType CustomerType { get; set; } = null!;

        public int? LinkedCustomerId { get; set; }

        [ForeignKey("LinkedCustomerId")]
        public virtual Customer? LinkedCustomer { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<SecuritySystem> SecuritySystems { get; set; } = new List<SecuritySystem>();
        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}