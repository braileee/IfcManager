using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public class PicklistGroup
    {
        public string GroupName { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}
