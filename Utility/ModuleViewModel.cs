namespace  LeavePortal.Utility
{
    public class ModuleViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleIcon { get; set; }
        public string ModuleUrl { get; set; }
        public List<SubModuleViewModel> SubModules { get; set; }
    }
}
