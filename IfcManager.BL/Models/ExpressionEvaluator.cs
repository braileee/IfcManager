using IfcManager.BL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public class ExpressionEvaluator
    {
        public static object Evaluate(ExpressionItem expression, object sourceParameterValue)
        {
            switch (expression.ExpressionFunctionType)
            {
                case ExpressionFunctionType.Undefined:
                    break;
                case ExpressionFunctionType.DelimetedByDotFirstPart:
                    if (sourceParameterValue is string strValue)
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
                case ExpressionFunctionType.SetTrueIfSourceEqualsTo:
                    if (sourceParameterValue == null)
                    {
                        return string.Empty;
                    }

                    return sourceParameterValue.ToString() == expression.Value;
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
