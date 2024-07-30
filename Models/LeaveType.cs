

using System.ComponentModel.DataAnnotations;

namespace LeavePortal.Models
{
    public class LeaveType
    {
        public LeaveType()
        {

        }
        public int LeaveTypeId { get; set; }
        [Required]
        [Display(Name ="Leave Type")]
        public string LeaveTypeName { get; set; }
        public bool Status { get; set; }

    }
}
