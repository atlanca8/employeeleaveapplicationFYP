using LeavePortal.Areas.Identity.Data;
using LeavePortal.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeavePortal.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public LoginController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult FortgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FortgotPassword(ForgetPasswordVM forget)
        {
            var obj = _context.Users.Where(x => x.Email == forget.Email).FirstOrDefault();        
            if(obj !=null)
            {
                return RedirectToAction("RestPassword", "Login", new { userid = obj.Id });
            }
            ModelState.AddModelError(string.Empty, "This user is not available in our system. Please provide the correct email address.");
            return View(obj);
        }
        public IActionResult RestPassword(string userid)
        {
            var obj = _context.Users.Where(x => x.Id == userid).FirstOrDefault();
            if(obj !=null)
            {
                ViewBag.userid = userid;
                return View();
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> RestPassword(ForgetPasswordVM forget)
        {
            var user = _context.Users.Where(x => x.Id == forget.id).FirstOrDefault();
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, forget.Password);

                if (result.Succeeded)
                {
                    return Redirect("/Identity/Account/Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }

            return View(forget);
        }
    }
}
