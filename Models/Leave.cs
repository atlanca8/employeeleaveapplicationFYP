using LeavePortal.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeavePortal.Models
{
    public class Leave
    {
        public Leave()
        {

        }
        public int LeaveId { get; set; }
        public int? LeaveTypeId { get; set; }
        public string? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LeaveHours { get; set; }
        public string Reason { get; set; }
        public int? Status { get; set; }
        public string? ApproverComments { get; set; }
        public string? ApplicationUserId { get; set; }
        public string? ApprovedById { get; set; }

    }
}
