using MicrostationIfcManager.Enums;

namespace MicrostationIfcManager.Models
{
    public class ExpressionItem
    {
        public string SourcePropertyName { get; set; }
        public string TargetPropertyName { get; set; }
        public ExpressionFunctionType ExpressionFunctionType { get; set; }
        public string Value { get; set; }
    }
}
