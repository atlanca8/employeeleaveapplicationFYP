

using LeavePortal.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace LeavePortal.Helpers
{
    public class ModulePermissionFilter : IAuthorizationFilter
    {
        private readonly string _moduleName;
        private readonly ApplicationDbContext _context;

        public ModulePermissionFilter(string moduleName, ApplicationDbContext context)
        {
            _moduleName = moduleName;
            _context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User.Identity.Name;

            // Check if the email is admin123@gmail.com and bypass authorization if true
            if (user == "Admin123@gmail.com")
            {
                return; // Authorized
            }

            var userId = _context.Users
                .Where(u => u.Email == user)
                .Select(x => x.Id)
                .FirstOrDefault();

            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleName == _moduleName);

            if (module == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = _context.UserPermissions
                .Any(up => up.UserId == userId && up.ModuleId == module.ModuleId);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }

    }

}
