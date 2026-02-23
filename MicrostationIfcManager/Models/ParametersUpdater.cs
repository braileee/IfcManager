using Bentley.DgnPlatformNET.Elements;
using MicrostationIfcManager.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicrostationIfcManager.Models
{
    public class ParametersUpdater
    {
        public ParametersUpdater(List<Element> elements, List<PropertyField> changeFields, List<PropertyField> allFields, List<ExpressionItem> expressionItems, List<ComposedPropertyItem> composedPropertyItems, List<PropertyValueMatch> propertyValueExactMatches)
        {
            Elements = elements;
            ChangedFields = changeFields;
            Fields = allFields;
            ExpressionItems = expressionItems;
            ComposedItems = composedPropertyItems;
            PropertyValueExactMatches = propertyValueExactMatches;
        }

        public List<Element> Elements { get; } = new List<Element>();
        public List<PropertyField> ChangedFields { get; } = new List<PropertyField>();
        public List<PropertyField> Fields { get; } = new List<PropertyField>();
        public List<ExpressionItem> ExpressionItems { get; } = new List<ExpressionItem>();

        public List<ComposedPropertyItem> ComposedItems { get; set; } = new List<ComposedPropertyItem>();
        public List<PropertyValueMatch> PropertyValueExactMatches { get; } = new List<PropertyValueMatch>();

        public void Update()
        {
            foreach (PropertyField field in ChangedFields)
            {
                foreach (Element element in Elements)
                {

                    element.SetValue(field.Name, field.Value?.ToString());
                }
            }

            ApplyExactMatches();

            ApplyExpressions();
            ApplyComposed();
        }

        private void ApplyExactMatches()
        {
            foreach (PropertyField changedField in ChangedFields)
            {
                string changedFieldValue = changedField.Value?.ToString() ?? string.Empty;

                PropertyValueMatch propertyValueMatch = PropertyValueExactMatches.FirstOrDefault(item => item.PropertyNameSource == changedField.Name && item.PropertyValueSource == changedFieldValue);

                if(propertyValueMatch == null)
                {
                    continue;
                }

                var field = Fields.FirstOrDefault(item => item.Name == propertyValueMatch.PropertyNameTarget);

                field.Value = propertyValueMatch.PropertyValueTarget;
                foreach (Element element in Elements)
                {
                    element.SetValue(field.Name, propertyValueMatch.PropertyValueTarget);
                }
            }
        }

        private void ApplyComposed()
        {
            foreach (PropertyField changedField in ChangedFields)
            {
                foreach (var composedItem in ComposedItems)
                {
                    List<string> propertyNamesToCompose = ComposedItemEvaluator.GetPropertyNames(composedItem.Formula);

                    if (!propertyNamesToCompose.Contains(changedField.Name))
                    {
                        continue;
                    }

                    PropertyField composingField = Fields.FirstOrDefault(item => item.Name == composedItem.ComposedPropertyName);

                    if (composingField == null)
                    {
                        continue;
                    }

                    List<PropertyField> fieldsToCompose = Fields.Where(item => propertyNamesToCompose.Contains(item.Name)).ToList();

                    Dictionary<string, string> propertyAndValuesToCompose = fieldsToCompose.ToDictionary(item => item.Name, item => item?.Value?.ToString());

                    string value = ComposedItemEvaluator.Resolve(composedItem.Formula, propertyAndValuesToCompose);

                    composingField.Value = value;

                    foreach (Element element in Elements)
                    {
                        element.SetValue(composingField.Name, value);
                    }
                }
            }
        }

        private void ApplyExpressions()
        {
            List<string> sourceParameterNames = ExpressionItems.Select(item => item.SourcePropertyName).Distinct().ToList();

            foreach (var changedField in ChangedFields)
            {
                if (!sourceParameterNames.Contains(changedField.Name))
                {
                    continue;
                }

                List<ExpressionItem> expressions = ExpressionItems.Where(item => item.SourcePropertyName == changedField.Name).ToList();

                foreach (var expression in expressions)
                {
                    PropertyField targetField = Fields.FirstOrDefault(f => f.Name == expression.TargetPropertyName);

                    if (targetField == null)
                    {
                        continue;
                    }


                    targetField.Value = ExpressionEvaluator.Evaluate(expression, changedField.Value);
                    targetField.CanBeEdited = false;
                    targetField.IsReadOnly = true;

                    foreach (Element element in Elements)
                    {
                        element.SetValue(targetField.Name, targetField.Value?.ToString());
                    }
                }
            }
        }

    }
}
