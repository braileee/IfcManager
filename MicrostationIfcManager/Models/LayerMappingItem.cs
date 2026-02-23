using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrostationIfcManager.Models
{
    public class LayerMappingItem
    {
        public string LayerName { get; set; }
        public Dictionary<string, string> PropertiesWithValues { get; set; } = new Dictionary<string, string>();
    }
}
