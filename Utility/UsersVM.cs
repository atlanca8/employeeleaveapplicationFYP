using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LeavePortal.Utility
{
    public class UsersVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string NRICNo { get; set; }
        public DateTime? StartDate { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string Department { get; set; }

    }
}
