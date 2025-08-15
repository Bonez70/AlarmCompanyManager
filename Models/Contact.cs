using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string? HomePhone { get; set; }

        [StringLength(15)]
        public string? BusinessPhone { get; set; }

        [StringLength(15)]
        public string? CellPhone { get; set; }

        [StringLength(100)]
        public string? EmailAddress { get; set; }

        public int ContactTypeId { get; set; }

        [ForeignKey("ContactTypeId")]
        public virtual ContactType ContactType { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}