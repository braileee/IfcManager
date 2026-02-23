using System.Collections.Generic;

namespace IfcValidator.Models
{
    public class PicklistGroup
    {
        public string GroupName { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}
