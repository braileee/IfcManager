using IfcManager.BL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public class ExpressionRule
    {
        public string SourcePropertyName { get; set; }
        public string TargetPropertyName { get; set; }
        public string Function { get; set; }
    }
}
