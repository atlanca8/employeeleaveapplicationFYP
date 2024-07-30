using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LeavePortal.Models;
using Microsoft.AspNetCore.Identity;

namespace LeavePortal.Areas.Identity.Data;


public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {

    }
    public string? Name { get; set; }
    public string? NRICNo { get; set; }
    public DateTime? StartDate { get; set; }
    
    public int? DepartmentId { get; set; }
    public int? AllocatedLeaves { get; set; }
    public bool? IsActive { get; set; }


    //public List<UserPermissions> UserPermissions { get; set; } = new List<UserPermissions>();
}

