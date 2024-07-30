using System.ComponentModel.DataAnnotations;

namespace LeavePortal.Utility
{
    public class AddLeavesVM
    {
        public int LeaveId { get; set; }
        [Required]
        public int? LeaveTypeId { get; set; }
        [Required]
        public string? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LeaveHours { get; set; }
        public string Reason { get; set; }
        public int? Status { get; set; }
        public string? ApplicationUserId { get; set; }
    }
}
