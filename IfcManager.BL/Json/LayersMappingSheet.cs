using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Json
{
    public sealed class LayersMappingSheet : OneSheetBase
    {
        public string LayerColumnName { get; set; }

        public LayersMappingSheet()
        {
            LayerColumnName = string.Empty;
        }
    }
}
