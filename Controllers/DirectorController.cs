using AspNetCoreHero.ToastNotification.Abstractions;
using LeavePortal.Areas.Identity.Data;
using LeavePortal.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeavePortal.Controllers
{
    public class DirectorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public DirectorController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;            
            _roleManager = roleManager;
            _userManager = userManager;

        }
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
                                where r.Name != "Admin"
                                select u).Count();
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
            ViewBag.PendingLeaves = pending;
            ViewBag.Rejected = canceled;
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
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
            join user in _context.Users on leave.ApplicationUserId equals user.Id
            join userroles in _context.UserRoles on user.Id equals userroles.UserId
            join roles in _context.Roles on userroles.RoleId equals roles.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.Pending &&
                         roles.Name=="Manager"
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
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join userroles in _context.UserRoles on user.Id equals userroles.UserId
                         join roles in _context.Roles on userroles.RoleId equals roles.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.Approved &&
                         roles.Name == "Manager"
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
            ViewBag.LeaveTypes = new SelectList(_context.LeaveType.ToList(), "LeaveTypeId", "LeaveTypeName");
            var l = _context.Leave.ToList();
            var leaves = from leave in _context.Leave
                         join type in _context.LeaveType on leave.LeaveTypeId equals type.LeaveTypeId
                         join user in _context.Users on leave.ApplicationUserId equals user.Id
                         join userroles in _context.UserRoles on user.Id equals userroles.UserId
                         join roles in _context.Roles on userroles.RoleId equals roles.Id
                         join approver in _context.Users on leave.ApprovedById equals approver.Id into approverGroup
                         from approver in approverGroup.DefaultIfEmpty()
                         where leave.Status == (int)Utility.Enums.LeaveStatusEnum.Rejected &&
                         roles.Name == "Manager"
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
