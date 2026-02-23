using IfcValidator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcValidator.Models
{
    public static class ExpressionEvaluator
    {
        public static string Evaluate(ExpressionFunctionType expressionFunctionType, object value)
        {
            switch (expressionFunctionType)
            {
                case ExpressionFunctionType.Undefined:
                    break;
                case ExpressionFunctionType.DelimetedByDotFirstPart:
                    if (value is string strValue)
                    {
                        if (string.IsNullOrEmpty(strValue))
                        {
                            return strValue;
                        }

                        var parts = strValue.Split('.');

                        if (parts.Length > 0)
                        {
                            return parts[0];
                        }
                    }
                    break;
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
