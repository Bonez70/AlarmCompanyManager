using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace AlarmCompanyManager.Models
{
    public class SecuritySystem
    {
        [Key]
        public int SecuritySystemId { get; set; }

        public int CustomerId { get; set; }

        [StringLength(20)]
        public string? CentralStationNumber { get; set; }

        public int? PanelTypeId { get; set; }

        public int? MonitoringTypeId { get; set; }

        public DateTime? MonitoringStartDate { get; set; }

        public DateTime? InstalledDate { get; set; }

        [StringLength(10)]
        public string? MasterSecurityCode { get; set; }

        [StringLength(50)]
        public string? CodeWord { get; set; }

        [StringLength(15)]
        public string? PolicePhone { get; set; }

        [StringLength(15)]
        public string? FireDeptPhone { get; set; }

        [StringLength(15)]
        public string? AmbulancePhone { get; set; }

        [StringLength(50)]
        public string? CityPermitNumber { get; set; }

        public DateTime? PermitDueDate { get; set; }

        [StringLength(500)]
        public string? AuthorityNotes { get; set; }

        public int? PrimaryCommunicatorId { get; set; }

        public int? SecondaryCommunicatorId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("PanelTypeId")]
        public virtual PanelType? PanelType { get; set; }

        [ForeignKey("MonitoringTypeId")]
        public virtual MonitoringType? MonitoringType { get; set; }

        [ForeignKey("PrimaryCommunicatorId")]
        public virtual Communicator? PrimaryCommunicator { get; set; }

        [ForeignKey("SecondaryCommunicatorId")]
        public virtual Communicator? SecondaryCommunicator { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();
        public virtual ICollection<CallListEntry> CallList { get; set; } = new List<CallListEntry>();
    }
}