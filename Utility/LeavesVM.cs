namespace LeavePortal.Utility
{
    public class LeavesVM
    {
        public int LeaveId { get; set; }
        public string? LeaveType { get; set; }
        public string? Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LeaveHours { get; set; }
        public string Reason { get; set; }
        public int? Status { get; set; }
        public string? ApproverComments { get; set; }
        public string? ApplicationUser { get; set; }
        public string? ApprovedBy { get; set; }
    }
}
