using IfcManager.BL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public class ExpressionItem
    {
        public string SourcePropertyName { get; set; }
        public string TargetPropertyName { get; set; }
        public ExpressionFunctionType ExpressionFunctionType { get; set; }
        public string Value { get; set; }
    }
}
