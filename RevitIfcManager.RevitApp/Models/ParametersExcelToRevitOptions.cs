using IfcManager.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitIfcManager.Models
{
    public class ParametersExcelToRevitOptions
    {
        public List<PropertySetItem> PropertySetItems { get; set; } = new List<PropertySetItem>();
    }
}
