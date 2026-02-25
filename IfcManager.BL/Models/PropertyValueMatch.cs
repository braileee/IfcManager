using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public class PropertyValueMatch
    {
        public string PropertyNameSource { get; set; }
        public string PropertyValueSource { get; set; }
        public string PropertyNameTarget { get; set; }
        public string PropertyValueTarget { get; set; }
    }
}
