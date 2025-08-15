using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlarmCompanyManager.Models
{
    public class Communicator
    {
        [Key]
        public int CommunicatorId { get; set; }

        public int CommunicatorTypeId { get; set; }

        [StringLength(50)]
        public string? Manufacturer { get; set; }

        [StringLength(50)]
        public string? ModelNumber { get; set; }

        [StringLength(20)]
        public string? RadioId { get; set; }

        [StringLength(15)]
        public string? IpAddress { get; set; }

        [StringLength(15)]
        public string? Gateway { get; set; }

        [StringLength(15)]
        public string? Subnet { get; set; }

        [StringLength(15)]
        public string? PhoneNumber1 { get; set; }

        [StringLength(15)]
        public string? PhoneNumber2 { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        [ForeignKey("CommunicatorTypeId")]
        public virtual CommunicatorType CommunicatorType { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<SecuritySystem> PrimarySecuritySystems { get; set; } = new List<SecuritySystem>();
        public virtual ICollection<SecuritySystem> SecondarySecuritySystems { get; set; } = new List<SecuritySystem>();
    }
}