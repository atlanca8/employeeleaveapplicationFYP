using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeavePortal.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HeadingAttribute : Attribute
    {
        internal HeadingAttribute(string heading)
        {
            this.Heading = heading;
        }
        public string Heading { get; private set; }
    }
}
