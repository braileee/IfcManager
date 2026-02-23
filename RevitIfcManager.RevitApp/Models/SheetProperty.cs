using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitIfcManager.Models
{
    public class SheetProperty
    {
        public string Name { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}
