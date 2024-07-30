using LeavePortal.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace LeavePortal.Models
{
    public class Department
    {
        public Department()
        {

        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
