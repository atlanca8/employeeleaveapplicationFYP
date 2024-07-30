

using LeavePortal.Areas.Identity.Data;
using LeavePortal.Models;
using LeavePortal.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeavePortal.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModulesController(ApplicationDbContext context)
        {
            _context = context;
        }
        #region Permissions
        [HttpGet]
        public IActionResult UpsertPermissions(string userId)
        {
            var modules = _context.Modules.Where(x=>x.ModuleIcons!="fas fa-home").ToList();

            var permissionsDictionary = new Dictionary<int, UserPermissions>();

            foreach (var module in modules)
            {
                var permission = _context.UserPermissions.FirstOrDefault(x => x.ModuleId == module.ModuleId && x.UserId == userId);
                if (permission != null)
                {
                    permissionsDictionary[module.ModuleId] = permission;
                }
            }

            var model = new PermissionVM
            {
                UserId = userId,
                Module = modules,
                ModulePermissions = permissionsDictionary
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpsertPermissionsPost(string userId, int moduleId, int status)
        {
            var permissions = _context.UserPermissions.Where(p => p.UserId == userId).ToList();
            if (moduleId == null || moduleId == 0)
            {
                return RedirectToAction("UpsertPermissions", new { userId });
            }
            if (status == 0)
            {
                foreach (var permission in permissions)
                {
                    if (permission.ModuleId == moduleId)
                    {
                        _context.UserPermissions.Remove(permission);

                    }
                }
            }
            if (status == 1)
            {
                var permission = new UserPermissions
                {
                    UserId = userId,
                    ModuleId = moduleId
                };
                _context.UserPermissions.Add(permission);
            }

            _context.SaveChanges();

            return RedirectToAction("UpsertPermissions", new { userId });
        }
        #endregion
    }
}
