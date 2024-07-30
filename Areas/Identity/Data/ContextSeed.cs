using LeavePortal.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;

namespace LeavePortal.Areas.Identity.Data
{
    public static class ContextSeed
    {
        public static async Task seedRolesAsync(RoleManager<IdentityRole> roleManager)
        {

            string[] roleNames = { "Admin", "Director", "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultAdmin = new ApplicationUser
            {
                UserName = "Admin123@gmail.com",
                Email = "Admin123@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "132435465"
            };
            if (userManager.Users.All(u => u.Id != defaultAdmin.Id))
            {
                try
                {
                    var user = await userManager.CreateAsync(defaultAdmin, "Admin123@");

                    if (user.Succeeded)
                    {
                        await userManager.AddToRoleAsync(defaultAdmin, "Admin");

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public static async Task SeedModulesAsync(ApplicationDbContext context)
        {
            var modules = new List<Modules>
{
        new Modules { ModuleId = 1, ModuleName = "Employees", ModuleUrl = "/Admin/Employees", ModuleIcons = "fas fa-users" },
    new Modules { ModuleId = 2, ModuleName = "Add Employee", ModuleUrl = "/Admin/AddEmployee", ModuleIcons = "", ParentModuleId = 1 },
    new Modules { ModuleId = 3, ModuleName = "Edit Employee", ModuleUrl = "/Admin/EditEmployee", ModuleIcons = "", ParentModuleId = 1 },
    new Modules { ModuleId = 4, ModuleName = "Delete Employee", ModuleUrl = "/Admin/DeleteEmployee", ModuleIcons = "", ParentModuleId = 1 },

    new Modules { ModuleId = 5, ModuleName = "Deparments", ModuleUrl = "/Departments/Index", ModuleIcons = "fas fa-building" },
    new Modules { ModuleId = 6, ModuleName = "Add Department", ModuleUrl = "/Departments/Create", ModuleIcons = "", ParentModuleId = 5 },
    new Modules { ModuleId = 7, ModuleName = "Edit Department", ModuleUrl = "/Departments/Edit", ModuleIcons = "", ParentModuleId = 5 },
    new Modules { ModuleId = 8, ModuleName = "Delete Department", ModuleUrl = "/Departments/DeleteConfirmed", ModuleIcons = "", ParentModuleId = 5 },

        new Modules { ModuleId = 9, ModuleName = "Leave Types", ModuleUrl = "/Admin/LeaveTypes", ModuleIcons = "fas fa-code-branch" },
    new Modules { ModuleId = 10, ModuleName = "Add LeaveType", ModuleUrl = "/Admin/AddLeaveType", ModuleIcons = "", ParentModuleId = 9 },
    new Modules { ModuleId = 11, ModuleName = "Edit LeaveType", ModuleUrl = "/Admin/EditLeaveType", ModuleIcons = "", ParentModuleId = 9 },
    new Modules { ModuleId = 12, ModuleName = "Delete LeaveType", ModuleUrl = "/Admin/DeleteConfirmed", ModuleIcons = "", ParentModuleId = 9 },

     new Modules { ModuleId = 13, ModuleName = "Leaves", ModuleUrl = "/Admin/NewLeaves", ModuleIcons = "fas fa-bed" },
     //new Modules { ModuleId = 14, ModuleName = "Export Users", ModuleUrl = "/Admin/Employess", ModuleIcons = "fas fa-clipboard" },
      // new Modules { ModuleId = 14, ModuleName = "Export Users", ModuleUrl = "/Admin/Employess", ModuleIcons = "", ParentModuleId = 1 },

          new Modules { ModuleId = 15, ModuleName = "Holidays", ModuleUrl = "/Admin/Holidays", ModuleIcons = "fas fa-umbrella-beach" },
    new Modules { ModuleId = 16, ModuleName = "Add Holiday", ModuleUrl = "/Admin/AddHoliday", ModuleIcons = "", ParentModuleId = 15 },
    new Modules { ModuleId = 17, ModuleName = "Edit Holiday", ModuleUrl = "/Admin/EditHoliday", ModuleIcons = "", ParentModuleId = 15 },
    new Modules { ModuleId = 18, ModuleName = "Delete Holiday", ModuleUrl = "/Admin/DeleteHoliday", ModuleIcons = "", ParentModuleId = 15 },
     //new Modules { ModuleId = 15, ModuleName = "User Home", ModuleUrl = "/User/Index", ModuleIcons = "fas fa-home" },
     //new Modules { ModuleId = 16, ModuleName = "Director Home", ModuleUrl = "/Director/Index", ModuleIcons = "fas fa-home" },
     //new Modules { ModuleId = 17, ModuleName = "Manager Home", ModuleUrl = "/Manager/Index", ModuleIcons = "fas fa-home" },
     //new Modules { ModuleId = 18, ModuleName = "Admin Home", ModuleUrl = "/Admin/Index", ModuleIcons = "fas fa-home" },
     new Modules { ModuleId = 19, ModuleName = "Profile", ModuleUrl = "/Identity/Account/Manage", ModuleIcons = "fas fa-user-check" },
};
            if (!context.Modules.Any())
            {
                try
                {
                    foreach (var module in modules)
                    {
                        context.Modules.Add(module);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}
