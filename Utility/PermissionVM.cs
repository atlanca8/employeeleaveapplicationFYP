

using LeavePortal.Models;

namespace LeavePortal.Utility
{
    public class PermissionVM
    {

        public string UserId { get; set; }
        public List<Modules> Module { get; set; }
        public Dictionary<int, UserPermissions> ModulePermissions { get; set; } = new Dictionary<int, UserPermissions>();
    }
}
