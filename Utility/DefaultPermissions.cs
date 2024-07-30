namespace LeavePortal.Utility
{
    public static class DefaultPermissions
    {
        public static class EmployeePermissions
        {
            public static readonly List<int> ModuleIds = new List<int>
            {
               
                19,  // Profile
            };
        }  
        public static class ManagerPermissions
        {
            public static readonly List<int> ModuleIds = new List<int>
            {

                19,  //Profile
            };
        } 
        public static class DirectorPermissions
        {
            public static readonly List<int> ModuleIds = new List<int>
            {

                19,  //Profile
            };
        }
    }
}
