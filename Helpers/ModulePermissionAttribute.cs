
using LeavePortal.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace LeavePortal.Helpers
{
    public class ModulePermissionAttribute : TypeFilterAttribute
    {
        public ModulePermissionAttribute(string moduleName) : base(typeof(ModulePermissionFilter))
        {
            Arguments = new object[] { moduleName };
        }
    }


}
