using LeavePortal.Utility.Attributes;
using System.ComponentModel;

namespace LeavePortal.Utility.Enums
{
    public enum LeaveStatusEnum
    {
        [Heading("Pending")]
        Pending = 1,

        [Heading("Manager Approved")]
        ManagerApproved = 2,

        [Heading("Manager Rejected")]
        ManagerRejected = 3, 
        
        [Heading("Approved")]
        Approved = 4,

        [Heading("Rejected")]
        Rejected = 5,
        
        [Heading("Canceled")]
        Canceled = 6,

    }
}
