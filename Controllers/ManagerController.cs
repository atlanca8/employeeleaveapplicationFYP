using AspNetCoreHero.ToastNotification.Abstractions;
using LeavePortal.Areas.Identity.Data;
using LeavePortal.Models;
using LeavePortal.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeavePortal.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public ManagerController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;

        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            var employees = from u in _context.Users
                            join ur in _context.UserRoles on u.Id equals ur.UserId
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where r.Name == "Employee" && u.DepartmentId == user.DepartmentId
                            select u;
            DateTime today = DateTime.Today;

            var employeeIds = employees.Select(u => u.Id).ToList();

            var onLeave = _context.Leave
     .Where(x =>
         (
             // Leaves that span multiple days
             (x.StartDate.HasValue && x.EndDate.HasValue && x.StartDate.Value <= today && x.EndDate.Value >= today) ||
             // Leaves that are single-day (StartDate and EndDate are null, and Date is today or Date is null)
             (!x.StartDate.HasValue && !x.EndDate.HasValue && (x.Date.HasValue && x.Date.Value.Date == today) || (!x.StartDate.HasValue && !x.EndDate.HasValue && !x.Date.HasValue))
         )
         && x.Status == (int)Utility.Enums.LeaveStatusEnum.Approved
         && employeeIds.Contains(x.ApplicationUserId) // Filter by specific user IDs
     )
     .Count();

            var OnBoardToday = employees.Count();
            var pending = _context.Leave
                .Where(x =>
                    (
                        // Leaves that span multiple days
                        (x.StartDate.HasValue && x.EndDate.HasValue && x.StartDate.Value <= today && x.EndDate.Value >= today) ||
                        // Leaves that are single-day (StartDate and EndDate are null, and Date is today or Date is null)
                        ((!x.StartDate.HasValue && !x.EndDate.HasValue && (x.Date.HasValue && x.Date.Value.Date == today)) || (!x.StartDate.HasValue && !x.EndDate.HasValue && !x.Date.HasValue))
                    )
                    && x.Status == (int)Utility.Enums.LeaveStatusEnum.Pending
                    && employeeIds.Contains(x.ApplicationUserId) // Filter by specific user IDs
                )
                .Count();
            var canceled = _context.Leave
       .Where(x =>
           (
               // Leaves that span multiple days
               (x.StartDate.HasValue && x.EndDate.HasValue && x.StartDate.Value <= today && x.EndDate.Value >= today) ||
               // Leaves that are single-day (StartDate and EndDate are null, and Date is today or Date is null)
               ((!x.StartDate.HasValue && !x.EndDate.HasValue && (x.Date.HasValue && x.Date.Value.Date == today)) || (!x.StartDate.HasValue && !x.EndDate.HasValue && !x.Date.HasValue))
           )
           && x.Status == (int)Utility.Enums.LeaveStatusEnum.Rejected
           && employeeIds.Contains(x.ApplicationUserId) // Filter by specific user IDs
       )
       .Count();

            ViewBag.OnLeaveToday = onLeave;
            ViewBag.OnBoardToday = OnBoardToday - onLeave; 
            ViewBag.PendingLeaves = pending;
            ViewBag.Rejected = canceled;


            if (user != null)
            {
                DateTime currentYearStart = new DateTime(DateTime.Today.Year, 1, 1);
                DateTime currentYearEnd = new DateTime(DateTime.Today.Year, 12, 31);
                var leavesTaken = _context.Leave
                    .Where(l => l.ApplicationUserId == user.Id && l.Date >= currentYearStart && l.Date <= currentYearEnd)
                    .ToList();
                int finaltaken = leavesTaken.Where(x => x.Status == (int)Utility.Enums.LeaveStatusEnum.Approved).ToList().Count();
                int allocatedLeaves = user.AllocatedLeaves ?? 0;
                int leavesLeft = allocatedLeaves - finaltaken;
                ViewBag.leavesleft = leavesLeft;
                ViewBag.leavestaken = leavesTaken.Count();
                ViewBag.approved = finaltaken;
                ViewBag.AllocatedLeaves = allocatedLeaves;
            }
            return View();
        }
        #region Leaves
        [HttpPost]
        public async Task<IActionResult> ChangeLeaveStatus(int id, int status, string? remarks)
        {

            var obj = await _context.Leave.FindAsync(id);
            if (obj != null)
            {
                var userId = _userManager.GetUserId(User);
                obj.ApprovedById = userId;
                obj.ApproverComments = remarks;
                obj.Status = status;
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


        public IActionResult NewLeaves()
        {
            var userId=_userManager.GetUserId(User);
            var deptId=_context.Users.Where(x=>x.Id==userId).Select(d=>d.DepartmentId).FirstOrDefault();    
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join userroles in _context.UserRoles on leave.ApplicationUserId equals userroles.UserId
                         join roles in _context.Roles on userroles.RoleId equals roles.Id
                         join d in _context.Department on user.DepartmentId equals deptId
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.Pending &&
                         roles.Name == "Employee"
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
        public IActionResult Approved()
        {
            var userId = _userManager.GetUserId(User);
            var deptId = _context.Users.Where(x => x.Id == userId).Select(d => d.DepartmentId).FirstOrDefault();
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
            join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
            join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join userroles in _context.UserRoles on leave.ApplicationUserId equals userroles.UserId
                         join roles in _context.Roles on userroles.RoleId equals roles.Id
                         join d in _context.Department on user.DepartmentId equals deptId
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.ManagerApproved &&
                         roles.Name == "Employee"
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
        public IActionResult Rejected()
        {
            var userId = _userManager.GetUserId(User);
            var deptId = _context.Users.Where(x => x.Id == userId).Select(d => d.DepartmentId).FirstOrDefault();
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join userroles in _context.UserRoles on leave.ApplicationUserId equals userroles.UserId
                         join roles in _context.Roles on userroles.RoleId equals roles.Id
                         join d in _context.Department on user.DepartmentId equals deptId
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.ManagerRejected &&
                         roles.Name == "Employee"
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
    
        public IActionResult MyLeaves()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.ApplicationUserId == userId
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
    }
}
