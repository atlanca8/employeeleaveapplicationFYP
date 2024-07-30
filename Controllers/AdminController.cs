using AspNetCoreHero.ToastNotification.Abstractions;
using LeavePortal.Areas.Identity.Data;
using LeavePortal.Helpers;
using LeavePortal.Models;
using LeavePortal.Utility;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeavePortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;

        }
        [Authorize]
        public IActionResult Index()
        {
            DateTime today = DateTime.Today;
            var onLeave = _context.Leave
       .Where(x =>
           (
               // Leaves that span multiple days
               (x.StartDate.HasValue && x.EndDate.HasValue && x.StartDate.Value <= today && x.EndDate.Value >= today) ||
               // Leaves that are single-day (StartDate and EndDate are null, and Date is today or Date is null)
               (!x.StartDate.HasValue && !x.EndDate.HasValue && (x.Date.HasValue && x.Date.Value.Date == today) || (!x.StartDate.HasValue && !x.EndDate.HasValue && !x.Date.HasValue))
           )
           && x.Status == (int)Utility.Enums.LeaveStatusEnum.Approved
       )
       .Count();

            var OnBoardToday = (from u in _context.Users
                                   join ur in _context.UserRoles on u.Id equals ur.UserId
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where r.Name != "Admin" select u ).Count();
            var pending = _context.Leave
         .Where(x =>
             (
                 // Leaves that span multiple days
                 (x.StartDate.HasValue && x.EndDate.HasValue && x.StartDate.Value <= today && x.EndDate.Value >= today) ||
                 // Leaves that are single-day (StartDate and EndDate are null, and Date is today or Date is null)
                 ((!x.StartDate.HasValue && !x.EndDate.HasValue && (x.Date.HasValue && x.Date.Value.Date == today)) || (!x.StartDate.HasValue && !x.EndDate.HasValue && !x.Date.HasValue))
             ) &&
             x.Status == (int)Utility.Enums.LeaveStatusEnum.Pending)
         .Count();

            var canceled = _context.Leave
         .Where(x =>
             (
                 // Leaves that span multiple days
                 (x.StartDate.HasValue && x.EndDate.HasValue && x.StartDate.Value <= today && x.EndDate.Value >= today) ||
                 // Leaves that are single-day (StartDate and EndDate are null, and Date is today or Date is null)
                 ((!x.StartDate.HasValue && !x.EndDate.HasValue && (x.Date.HasValue && x.Date.Value.Date == today)) || (!x.StartDate.HasValue && !x.EndDate.HasValue && !x.Date.HasValue))
             ) &&
             x.Status == (int)Utility.Enums.LeaveStatusEnum.Rejected)
         .Count();
            ViewBag.OnLeaveTody = onLeave;
            ViewBag.OnBoardToday = OnBoardToday - onLeave;
            ViewBag.PendingLeaves= pending; 
            ViewBag.Rejected=canceled;
            return View();
        }

        #region Employees

        [ModulePermission("Employees")]
        public IActionResult Employees()
        {
            string userid = _userManager.GetUserId(User);
            var getEmployee = (from u in _context.Users
                               join ur in _context.UserRoles on u.Id equals ur.UserId
                               join r in _context.Roles on ur.RoleId equals r.Id
                               join d in _context.Department on u.DepartmentId equals d.Id
                               where r.Name != "Admin"
                               select new UsersVM()
                               {
                                   Name = u.Name,
                                   Email = u.Email,
                                   Role = r.Name,
                                   Department = d.Name,
                                   NRICNo = u.NRICNo,
                                   StartDate = u.StartDate,
                                   IsActive = (bool)u.IsActive,
                                   Mobile = u.PhoneNumber,
                                   Id = u.Id
                               }).ToList();
            return View(getEmployee);
        }
        [ModulePermission("Add Employee")]
        public IActionResult AddEmployee()
        {
            var getEmployee = _context.Department.ToList();
            var getRoles = _context.Roles.Where(x => x.Name != "Admin").ToList();
            if (getEmployee == null || getEmployee.Count == 0)
            {

                ViewBag.Department = null;
            }
            else
            {

                ViewBag.Department = new SelectList(_context.Department, "Id", "Name");
                ViewBag.Roles = new SelectList(getRoles, "Id", "Name");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(User obj)
        {
            var available = _userManager.Users.Where(x => x.Email == obj.Email.ToString()).FirstOrDefault();
            if (available == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Name = obj.Name,
                    NRICNo = obj.NRICNo,
                    Email = obj.Email,
                    UserName = obj.Email,
                    StartDate = obj.startDate,
                    AllocatedLeaves = obj.AllocatedLeaves,
                    PhoneNumber = obj.MobileNumber,
                    DepartmentId = obj.DepartmentID,
                    IsActive = obj.IsActive,

                };
                var result = await _userManager.CreateAsync(user, obj.Password);
                var role = _context.Roles.Where(x => x.Id == obj.UserRole).Select(x => x.Name).FirstOrDefault();
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                    await _userManager.AddToRoleAsync(user, role);
                    await AddDefaultPermissionsAsync(user.Id, role);
                    return RedirectToAction("Employees");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User Email already Taken!");

                return View();

            }

            return RedirectToAction("Employees");
        }

        [ModulePermission("Edit Employee")]
        [HttpGet]
        public async Task<IActionResult> EditEmployee(string id)
        {
            var getRoles = _context.Roles.Where(x => x.Name != "Admin").ToList();
            ViewBag.Department = new SelectList(_context.Department, "Id", "Name");
            ViewBag.Roles = new SelectList(getRoles, "Id", "Name");
            var user = _context.Users
                         .Where(u => u.Id == id)
                         .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                         .Join(_context.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur.u, r })
                         .Select(result => new User
                         {
                             Name = result.u.Name,
                             NRICNo = result.u.NRICNo,
                             Email = result.u.Email,
                             AllocatedLeaves = result.u.AllocatedLeaves,
                             IsActive = result.u.IsActive.HasValue && result.u.IsActive.Value,
                             startDate = result.u.StartDate,
                             DepartmentID = result.u.DepartmentId ?? 0,
                             MobileNumber = result.u.PhoneNumber,
                             UserRole = result.r.Id
                         })
                         .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(string id, User model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.Name = model.Name;
            user.NRICNo = model.NRICNo;
            user.PhoneNumber = model.MobileNumber;
            user.DepartmentId = model.DepartmentID;
            user.AllocatedLeaves = model.AllocatedLeaves;
            user.IsActive = model.IsActive;

            // Update user role
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            if (currentRole != model.UserRole)
            {
                if (currentRole != null)
                {
                    var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                    if (!removeRoleResult.Succeeded)
                    {
                        foreach (var error in removeRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                var role = _context.Roles.Where(x => x.Id == model.UserRole).Select(x => x.Name).FirstOrDefault();
                var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Employees");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        [ModulePermission("Delete Employee")]
        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var leaves = _context.Leave.Where(x => x.ApplicationUserId == id).ToList();
            foreach(var item in leaves)
            {
                _context.Leave.Remove(item);
                _context.SaveChanges();
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return new JsonResult("Employee Delete Successfully");
            }

            else
            {
                return new JsonResult("System Error! Employee is not Delete Successfully");

            }
        }
        #endregion

        #region Default Permissions
        private async Task AddDefaultPermissionsAsync(string userId, string role)
        {
            List<int> defaultModuleIds;

            switch (role)
            {
                case "Employee":
                    defaultModuleIds = DefaultPermissions.EmployeePermissions.ModuleIds;
                    break;
                case "Manager":
                    defaultModuleIds = DefaultPermissions.ManagerPermissions.ModuleIds;
                    break;
                case "Director":
                    defaultModuleIds = DefaultPermissions.DirectorPermissions.ModuleIds;
                    break;
                default:
                    throw new ArgumentException("Invalid role");
            }

            var defaultPermissions = defaultModuleIds
                .Select(moduleId => new UserPermissions
                {
                    UserId = userId,
                    ModuleId = moduleId
                })
                .ToList();

            _context.UserPermissions.AddRange(defaultPermissions);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Leave Type

        [ModulePermission("Leave Types")]
        public IActionResult LeaveTypes()
        {
            var obj = _context.LeaveType.ToList();
            return View(obj);       
        }

        [ModulePermission("Add LeaveType")]
        public IActionResult AddLeaveType()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddLeaveType(LeaveType type)
        {
            if (ModelState.IsValid)
            {
                _context.LeaveType.Add(type);
                _context.SaveChanges();
                return RedirectToAction("LeaveTypes");
            }
            return View(type);
        }
        [ModulePermission("Edit LeaveType")]
        public IActionResult EditLeaveType(int? id)
        {
            var obj = _context.LeaveType.Find(id);
            if(obj !=null)
            {
                return View(obj);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult EditLeaveType(int id, LeaveType type)
        {
            if(type !=null)
            {
                _context.Update(type);
                _context.SaveChanges();
                return RedirectToAction("LeaveTypes");
            }
            return View(type);
        }

        [ModulePermission("Delete LeaveType")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var type = await _context.LeaveType.FindAsync(id);
            if (type != null)
            {
                _context.LeaveType.Remove(type);
            }
            var leaves = _context.Leave.Where(x => x.LeaveTypeId == id).ToList();
            foreach (var item in leaves)
            {
                _context.Leave.Remove(item);
                _context.SaveChanges();
            }
            int check = await _context.SaveChangesAsync();

            if (check == 1)
            {
                return new JsonResult("Leave Type Delete Successfully");
            }
            else
            {
                return new JsonResult("System Error! Leave Type is not Delete.");

            }
        }
        #endregion

        #region Apply Leave
        public IActionResult ApplyLeave()
        {
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyLeave(AddLeavesVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.Duration == "single_day")
                {
                    model.Date = model.Date;
                    model.StartDate = null;
                    model.EndDate = null;
                    model.LeaveHours = null;
                }
                else if (model.Duration == "hours")
                {
                    model.Date = model.Date;
                    model.StartDate = null;
                    model.EndDate = null;
                }
                else if (model.Duration == "multiple_days")
                {
                    model.Date = model.StartDate;
                    model.LeaveHours = null;
                }
                Leave obj = new Leave()
                {
                    StartDate = model.StartDate,
                    LeaveTypeId = model.LeaveTypeId,
                    EndDate = model.EndDate,
                    Reason = model.Reason,
                    LeaveHours = model.LeaveHours,
                    Duration = model.Duration,
                    Date = model.Date,
                    ApplicationUserId = _userManager.GetUserId(User),
                    Status = (int)Utility.Enums.LeaveStatusEnum.Pending,
                };
                // Assuming you have the current user's ID

                _context.Leave.Add(obj);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Manager"))
                {
                    return RedirectToAction("MyLeaves","Manager");
                }  
                if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("MyLeaves","User");
                } 
                if (User.IsInRole("Director"))
                {
                    return RedirectToAction("Leaves","User");
                }
                return RedirectToAction("ApplyLeave");
            }

            ViewBag.LeaveTypes = new SelectList(await _context.LeaveType.ToListAsync(), "LeaveTypeId", "LeaveTypeName", model.LeaveTypeId);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeLeaveStatus(int id,int status,string? remarks)
        {
        
            var obj = await _context.Leave.FindAsync(id);
            if (obj != null)
            {
                var userId = _userManager.GetUserId(User);
                obj.ApprovedById = userId;
                obj.Status= status; 
                obj.ApproverComments= remarks; 
                _context.Leave.Update(obj);
            }

            int check = await _context.SaveChangesAsync();

            if (check == 1)
            {
                return new JsonResult("changed Successfully");
            }
            else
            {
                return new JsonResult("System Error! Leave status not changed.");

            }
        }
        #endregion

        #region Leaves
        [ModulePermission("Leaves")]
        public IActionResult NewLeaves()
        {
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status==(int)Utility.Enums.LeaveStatusEnum.Pending
                         select new LeavesVM
                         {
                             LeaveId = leave.LeaveId,
                             LeaveType = type.LeaveTypeName, 
                             Duration = leave.Duration,
                             StartDate = leave.StartDate,
                             Date = leave.Date,
                             EndDate = leave.EndDate,
                             LeaveHours = leave.LeaveHours,
                             Reason = leave.Reason,
                             Status = leave.Status,
                             ApplicationUser = user.Name,
                             ApprovedBy = approver != null ? approver.Name : null 
                         };
            return View(leaves);
        }
        [ModulePermission("Leaves")]
        public IActionResult Approved()
        {
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.Approved || leave.Status == (int)Utility.Enums.LeaveStatusEnum.ManagerApproved
                         select new LeavesVM
                         {
                             LeaveId = leave.LeaveId,
                             LeaveType = type.LeaveTypeName,
                             Duration = leave.Duration,
                             StartDate = leave.StartDate,
                             Date = leave.Date,
                             EndDate = leave.EndDate,
                             LeaveHours = leave.LeaveHours,
                             Reason = leave.Reason,
                             Status = leave.Status,
                             ApplicationUser = user.Name,
                             ApprovedBy = approver != null ? approver.Name : null
                         };
            return View(leaves);
        }
        [ModulePermission("Leaves")]
        public IActionResult Rejected()
        {
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.Rejected || leave.Status == (int)Utility.Enums.LeaveStatusEnum.ManagerRejected
                         select new LeavesVM
                         {
                             LeaveId = leave.LeaveId,
                             LeaveType = type.LeaveTypeName,
                             Duration = leave.Duration,
                             StartDate = leave.StartDate,
                             Date = leave.Date,
                             EndDate = leave.EndDate,
                             LeaveHours = leave.LeaveHours,
                             Reason = leave.Reason,
                             Status = leave.Status,
                             ApplicationUser = user.Name,
                             ApprovedBy = approver != null ? approver.Name : null
                         };
            return View(leaves);
        }

        #endregion

        #region Holidays

        [ModulePermission("Holidays")]
        public IActionResult Holidays()
        {
            var obj = _context.Holiday.ToList();
            return View(obj);
          
        }

        [ModulePermission("Add Holiday")]
        public IActionResult AddHoliday()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddHoliday(Holiday obj)
        {
            if (ModelState.IsValid)
            {
                 _context.Add(obj);
                 _context.SaveChanges();
                return RedirectToAction("Holidays");
            }
            return View(obj);
        }
        [ModulePermission("Edit Holiday")]
        public IActionResult EditHoliday(int? id)
        {
            var obj =  _context.Holiday.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult EditHoliday(int id, Holiday obj)
        {
            if (id != obj.Id)
            {
                return NotFound();
            }

             _context.Update(obj);
             _context.SaveChanges();
            return RedirectToAction("Holidays");           
        }

        [ModulePermission("Delete Holiday")]
        [HttpPost]
        public async Task<IActionResult> DeleteHoliday(int id)
        {

            if (_context.Holiday == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Holiday'  is null.");
            }
            var obj = await _context.Holiday.FindAsync(id);
            if (obj != null)
            {
                _context.Holiday.Remove(obj);
            }

            int check = await _context.SaveChangesAsync();

            if (check == 1)
            {
                return new JsonResult("Holiday Delete Successfully");
            }
            else
            {
                return new JsonResult("System Error! Holiday is not Delete.");

            }
        }
        #endregion
    }
}
