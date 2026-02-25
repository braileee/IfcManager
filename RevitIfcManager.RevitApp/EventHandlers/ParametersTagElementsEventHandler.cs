using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using IfcManager.BL.Models;
using PSURevitApps.Core;
using PSURevitApps.Core.Models;
using RevitIfcManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitIfcManager.EventHandlers
{
    public class ParametersTagElementsEventHandler : RevitEventWrapper<ParametersTagElementsOptions>
    {
        public override void Execute(UIApplication app, ParametersTagElementsOptions options)
        {
            try
            {
                using (Transaction transaction = new Transaction(app.ActiveUIDocument.Document, "Change values"))
                {
                    transaction.Start();

                    foreach (PropertyField field in options.ChangedFields)
                    {
                        foreach (var element in options.Elements)
                        {
                            Parameter parameter = element.LookupParameter(field.Name);

                            if (parameter == null)
                            {
                                continue;
                            }

                            parameter.TryParseAndSet(field?.Value?.ToString());
                        }
                    }

                    ApplyExpressions(options);
                    ApplyComposed(options);
                    ApplyExactMatches(options);
                    transaction.Commit();
                }
            }
            catch (Exception exception)
            {
            }
        }

        private void ApplyComposed(ParametersTagElementsOptions options)
        {
            foreach (PropertyField changedField in options.ChangedFields)
            {
                foreach (var composedItem in options.ComposedItems)
                {
                    List<string> propertyNamesToCompose = ComposedItemEvaluator.GetPropertyNames(composedItem.Formula);

                    if (!propertyNamesToCompose.Contains(changedField.Name))
                    {
                        continue;
                    }

                    PropertyField composingField = options.Fields.FirstOrDefault(item => item.Name == composedItem.ComposedPropertyName);

                    if(composingField == null)
                    {
                        continue;
                    }

                    List<PropertyField> fieldsToCompose = options.Fields.Where(item => propertyNamesToCompose.Contains(item.Name)).ToList();

                    Dictionary<string, string> propertyAndValuesToCompose = fieldsToCompose.ToDictionary(item => item.Name, item => item?.Value?.ToString());

                    string value = ComposedItemEvaluator.Resolve(composedItem.Formula, propertyAndValuesToCompose);

                    composingField.Value = value;

                    foreach (Element element in options.Elements)
                    {
                        Parameter parameter = element.LookupParameter(composingField.Name);

                        if (parameter != null)
                        {
                            parameter.TryParseAndSet(value);
                        }
                    }
                }
            }
        }

        private void ApplyExactMatches(ParametersTagElementsOptions options)
        {
            foreach (PropertyField changedField in options.ChangedFields)
            {
                string changedFieldValue = changedField.Value?.ToString() ?? string.Empty;

                PropertyValueMatch propertyValueMatch = options.PropertyValueExactMatches.FirstOrDefault(item => item.PropertyNameSource == changedField.Name && item.PropertyValueSource == changedFieldValue);

                if (propertyValueMatch == null)
                {
                    continue;
                }

                var field = options.Fields.FirstOrDefault(item => item.Name == propertyValueMatch.PropertyNameTarget);

                field.Value = propertyValueMatch.PropertyValueTarget;

                foreach (Element element in options.Elements)
                {
                    Parameter parameter = element.LookupParameter(field.Name);

                    if (parameter != null)
                    {
                        parameter.TryParseAndSet(propertyValueMatch.PropertyValueTarget);
                    }
                }
            }
        }

        private void ApplyExpressions(ParametersTagElementsOptions options)
        {
            List<string> sourceParameterNames = options.Expressions.Select(item => item.SourcePropertyName).Distinct().ToList();

            foreach (PropertyField changedField in options.ChangedFields)
            {
                if (!sourceParameterNames.Contains(changedField.Name))
                {
                    continue;
                }

                List<ExpressionItem> relatedExpressions = options.Expressions.Where(item => item.SourcePropertyName == changedField.Name).ToList();

                foreach (var expression in relatedExpressions)
                {
                    PropertyField targetField = options.Fields.FirstOrDefault(f => f.Name == expression.TargetPropertyName);

                    if (targetField == null)
                    {
                        continue;
                    }

                    try
                    {
                        targetField.Value = ExpressionEvaluator.Evaluate(expression, changedField.Value);
                        targetField.CanBeEdited = false;
                        targetField.IsReadOnly = true;

                        foreach (Element element in options.Elements)
                        {
                            Parameter parameter = element.LookupParameter(targetField.Name);

                            if (parameter != null)
                            {
                                parameter.TryParseAndSet(targetField.Value.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }
}
