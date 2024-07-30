using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LeavePortal.Models
{
    public class Modules
    {
        [Key]
        public int ModuleId {  get; set; }
        [Required]
        [Display(Name ="Module Name")]
        public string ModuleName { get; set; }
        public string? ModuleUrl { get; set; }
        public string? ModuleIcons { get; set; }
        [JsonIgnore]
        public int? ParentModuleId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Modules ParentModule { get; set; }
        [JsonIgnore]
        public List<Modules> SubModules { get; set; } = new List<Modules>();

    }
}
