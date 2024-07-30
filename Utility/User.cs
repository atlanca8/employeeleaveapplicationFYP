using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LeavePortal.Utility
{
    public class User
    {
        [Display(Name = "Employee Name")]
        [Required]
        public string Name { get; set; }
    

        [Display(Name = "Company Email")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "NRIC No")]
        [Required]
        public string NRICNo { get; set; }
        
        [Display(Name = "Total Annual Leaves ")]
        [Required]
        public int? AllocatedLeaves { get; set; }

        [Display(Name = "Start Date")]
        [Required]
        public DateTime? startDate { get; set; }

        [Display(Name = "User Password")]
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]).{8,}$", ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. It should be at least 8 characters long.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "User Email ")]
        [Required]
        public string UserId { get; set; }

        [Display(Name = "Employee Postion")]
        [Required]
        public string UserRole { get; set; } 
        
        [Display(Name = "Active")]
        [Required]
        public bool IsActive { get; set; }


        [Display(Name = "Mobile Number")]
        [Required]
        public string MobileNumber { get; set; }

        [Display(Name = "Department")]
        [Required]
        public int DepartmentID { get; set; }
    }
}
