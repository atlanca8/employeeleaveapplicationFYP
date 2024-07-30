using LeavePortal.Areas.Identity.Data;
using LeavePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LeavePortal.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Modules>()
        .Property(m => m.ModuleId)
        .ValueGeneratedNever();

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<Department> Department { get; set; }
    public DbSet<Leave> Leave { get; set; }
    public DbSet<LeaveType> LeaveType { get; set; }
    public DbSet<Modules> Modules { get; set; }
    public DbSet<UserPermissions> UserPermissions { get; set; }
    public DbSet<Holiday> Holiday { get; set; }
}
