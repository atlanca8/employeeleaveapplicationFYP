using LeavePortal.Areas.Identity.Data;
using LeavePortal.Models;
using LeavePortal.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LeavePortal.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                DateTime currentYearStart = new DateTime(DateTime.Today.Year, 1, 1);
                DateTime currentYearEnd = new DateTime(DateTime.Today.Year, 12, 31);

                var leavesTaken = _context.Leave
                    .Where(l => l.ApplicationUserId == user.Id && l.Date >= currentYearStart && l.Date <= currentYearEnd)
                    .ToList();

                int finaltaken = leavesTaken.Count(x => x.Status == (int)Utility.Enums.LeaveStatusEnum.Approved);
                int allocatedLeaves = user.AllocatedLeaves ?? 0;
                int leavesLeft = allocatedLeaves - finaltaken;

                // Assign values to ViewBag properties
                ViewBag.leavesleft = leavesLeft;
                ViewBag.leavestaken = leavesTaken.Count();
                ViewBag.approved = finaltaken;
                ViewBag.AllocatedLeaves = allocatedLeaves;
            }

                return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeLeaveStatus(int id, int status, string? remarks)
        {

            var obj = await _context.Leave.FindAsync(id);
            if (obj != null)
            {
                var userId = _userManager.GetUserId(User);
                obj.ApprovedById = userId;
                obj.Status = status;
                obj.ApproverComments = remarks;
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
                         where leave.ApplicationUserId==userId
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
