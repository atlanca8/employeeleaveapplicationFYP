

using LeavePortal.Areas.Identity.Data;
using LeavePortal.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavePortal.Models
{
    public class UserPermissions
    {
        [Key]
        public int UserPermissionId { get; set; }
        public string UserId { get; set; }

        public int ModuleId { get; set; }
        [ValidateNever]
        [ForeignKey("ModuleId")]
        public Modules Modules { get; set; }
        [ValidateNever]
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
