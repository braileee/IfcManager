using IfcManager.BL.Enums;
using IfcManager.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Xbim.Ifc2x3.MeasureResource;

namespace IfcValidator.Models
{
    public class IfcFileDataFilter
    {
        public IfcFileDataFilter(List<IfcFile> ifcFiles, List<PropertySetItem> propertySetItems, List<PicklistGroup> picklistGroups)
        {
            IfcFiles = ifcFiles;
            PropertySetItems = propertySetItems;
        }

        public List<IfcFile> IfcFiles { get; } = new List<IfcFile>();
        public List<PropertySetItem> PropertySetItems { get; } = new List<PropertySetItem>();

        public List<IfcFile> GetPerPropertySetItems()
        {
            return GetFilteredIfcFiles();
        }

        private List<IfcFile> GetFilteredIfcFiles()
        {
            List<IfcFile> filteredIfcFiles = new List<IfcFile>();

            foreach (IfcFile ifcFile in IfcFiles)
            {
                IfcFile filteredIfcFile = new IfcFile
                {
                    FilePath = ifcFile.FilePath,
                    IfcElements = new List<IfcElement>()
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement ifcElementUpdated = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag
                    };

                    foreach (IfcProperty ifcProperty in ifcElement.IfcProperties)
                    {
                        PropertySetItem propertySetItem = PropertySetItems.FirstOrDefault(propertySetItem => propertySetItem.PropertySetName == ifcProperty.PropertySetName);

                        if (propertySetItem == null)
                        {
                            continue;
                        }

                        PropertyItem propertyItem = propertySetItem.PropertyDefinitions.FirstOrDefault(item => item.PropertyName == ifcProperty.PropertyName);

                        if (propertyItem == null)
                        {
                            continue;
                        }

                        ifcElementUpdated.IfcProperties.Add(ifcProperty);
                    }

                    filteredIfcFile.IfcElements.Add(ifcElementUpdated);
                }

                filteredIfcFiles.Add(filteredIfcFile);
            }

            return filteredIfcFiles;
        }

        public List<IfcFile> GetWithEmptyValues(List<IfcFile> filteredIfcFiles)
        {
            List<IfcFile> emptyPropertyFiles = new List<IfcFile>();

            foreach (IfcFile ifcFile in filteredIfcFiles)
            {
                IfcFile filteredIfcFile = new IfcFile
                {
                    FilePath = ifcFile.FilePath,
                    IfcElements = new List<IfcElement>()
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement ifcElementUpdated = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag
                    };

                    foreach (IfcProperty ifcProperty in ifcElement.IfcProperties)
                    {
                        PropertySetItem propertySetItem = PropertySetItems.FirstOrDefault(propertySetItem => propertySetItem.PropertySetName == ifcProperty.PropertySetName);

                        if (propertySetItem == null)
                        {
                            continue;
                        }

                        PropertyItem propertyItem = propertySetItem.PropertyDefinitions.FirstOrDefault(item => item.PropertyName == ifcProperty.PropertyName);

                        if (propertyItem == null)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(ifcProperty?.Value?.ToString()))
                        {
                            ifcElementUpdated.IfcProperties.Add(ifcProperty);
                        }
                    }

                    if (ifcElementUpdated.IfcProperties.Count > 0)
                    {
                        filteredIfcFile.IfcElements.Add(ifcElementUpdated);
                    }
                }

                emptyPropertyFiles.Add(filteredIfcFile);
            }

            return emptyPropertyFiles;
        }

        public List<IfcFile> GetPicklistCheck(List<IfcFile> ifcFiles, List<PicklistGroup> picklistGroups)
        {
            List<IfcFile> picklistCheckFiles = new List<IfcFile>();

            foreach (IfcFile ifcFile in ifcFiles)
            {
                IfcFile filteredIfcFile = new IfcFile
                {
                    FilePath = ifcFile.FilePath,
                    IfcElements = new List<IfcElement>()
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement ifcElementUpdated = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag
                    };

                    foreach (IfcProperty ifcProperty in ifcElement.IfcProperties)
                    {
                        PicklistGroup picklistGroup = picklistGroups.FirstOrDefault(item => item.GroupName == ifcProperty.PropertyName);

                        if (picklistGroup == null)
                        {
                            continue;
                        }

                        PropertySetItem propertySetItem = PropertySetItems.FirstOrDefault(propertySetItem => propertySetItem.PropertySetName == ifcProperty.PropertySetName);

                        if (propertySetItem == null)
                        {
                            continue;
                        }

                        PropertyItem propertyItem = propertySetItem.PropertyDefinitions.FirstOrDefault(item => item.PropertyName == ifcProperty.PropertyName);

                        if (propertyItem == null)
                        {
                            continue;
                        }

                        IfcProperty ifcPropertyUpdated = new IfcProperty
                        {
                            IsValueFromPicklist = false,
                            PropertyName = ifcProperty.PropertyName,
                            PropertySetName = ifcProperty.PropertySetName,
                            Value = ifcProperty.Value,
                        };

                        string value = ifcPropertyUpdated?.Value?.ToString();

                        ifcPropertyUpdated.IsValueFromPicklist = picklistGroup.Values.Contains(value);

                        if (!ifcPropertyUpdated.IsValueFromPicklist)
                        {
                            ifcElementUpdated.IfcProperties.Add(ifcPropertyUpdated);
                        }
                    }

                    if (ifcElementUpdated.IfcProperties.Count > 0)
                    {
                        filteredIfcFile.IfcElements.Add(ifcElementUpdated);
                    }
                }

                picklistCheckFiles.Add(filteredIfcFile);
            }

            return picklistCheckFiles;
        }

        public List<IfcFile> GetNonMatchData(List<IfcFile> ifcFiles, List<PropertyValueMatch> propertyValueMatches)
        {
            List<IfcFile> updatedFiles = new List<IfcFile>();

            foreach (IfcFile ifcFile in ifcFiles)
            {
                IfcFile filteredIfcFile = new IfcFile
                {
                    FilePath = ifcFile.FilePath,
                    IfcElements = new List<IfcElement>()
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement ifcElementUpdated = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag,
                    };

                    foreach (IfcProperty ifcProperty in ifcElement.IfcProperties)
                    {
                        string assignedSourceValue = ifcProperty?.Value?.ToString();

                        List<PropertyValueMatch> sourcePropertyValueMatches = propertyValueMatches.Where(item => item.PropertyNameSource == ifcProperty.PropertyName && item.PropertyValueSource == assignedSourceValue).ToList();

                        if (sourcePropertyValueMatches.Count == 0)
                        {
                            continue;
                        }

                        PropertySetItem propertySetItem = PropertySetItems.FirstOrDefault(propertySetItem => propertySetItem.PropertySetName == ifcProperty.PropertySetName);

                        if (propertySetItem == null)
                        {
                            continue;
                        }

                        PropertyItem propertyItem = propertySetItem.PropertyDefinitions.FirstOrDefault(item => item.PropertyName == ifcProperty.PropertyName);

                        if (propertyItem == null)
                        {
                            continue;
                        }


                        IfcProperty ifcPropertyTarget = ifcElement.IfcProperties.FirstOrDefault(item => item.PropertyName == propertyValueMatches[0].PropertyNameTarget);
                        string assignedTargetProperty = ifcPropertyTarget?.Value?.ToString();

                        IfcProperty ifcPropertyUpdated = new IfcProperty
                        {
                            PropertyName = ifcPropertyTarget.PropertyName,
                            PropertySetName = ifcPropertyTarget.PropertySetName,
                            Value = ifcPropertyTarget.Value,
                        };

                        List<string> sourceValues = sourcePropertyValueMatches.Select(item => item.PropertyValueSource).ToList();
                        List<string> targetValues = sourcePropertyValueMatches.Select(item => item.PropertyValueTarget).ToList();

                        if (!targetValues.Contains(assignedTargetProperty))
                        {
                            ifcElementUpdated.IfcProperties.Add(ifcPropertyUpdated);
                        }
                    }

                    if (ifcElementUpdated.IfcProperties.Count > 0)
                    {
                        filteredIfcFile.IfcElements.Add(ifcElementUpdated);
                    }
                }

                updatedFiles.Add(filteredIfcFile);
            }

            return updatedFiles;
        }

        public List<IfcFile> GetWrongExpressions(List<IfcFile> ifcFiles, List<ExpressionItem> expressions)
        {
            List<IfcFile> updatedFiles = new List<IfcFile>();

            foreach (IfcFile ifcFile in ifcFiles)
            {
                IfcFile filteredIfcFile = new IfcFile
                {
                    FilePath = ifcFile.FilePath,
                    IfcElements = new List<IfcElement>()
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement ifcElementUpdated = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag,
                    };

                    foreach (IfcProperty sourceProperty in ifcElement.IfcProperties)
                    {
                        string assignedSourceValue = sourceProperty?.Value?.ToString();

                        List<ExpressionItem> expressionItems = expressions.Where(expression => expression.SourcePropertyName == sourceProperty.PropertyName).ToList();

                        foreach (ExpressionItem expression in expressionItems)
                        {
                            IfcProperty targetProperty = ifcElement.IfcProperties.FirstOrDefault(item => item.PropertyName == expression.TargetPropertyName);

                            if (targetProperty == null)
                            {
                                continue;
                            }

                            string targetValue = targetProperty?.Value?.ToString();

                            object evaluatedValue = ExpressionEvaluator.Evaluate(expression, sourceProperty?.Value);
                            string evaluatedValueString = evaluatedValue?.ToString();

                            if (targetValue != evaluatedValueString)
                            {
                                ifcElementUpdated.IfcProperties.Add(sourceProperty);
                                ifcElementUpdated.IfcProperties.Add(targetProperty);
                            }
                        }
                    }

                    if (ifcElementUpdated.IfcProperties.Count > 0)
                    {
                        filteredIfcFile.IfcElements.Add(ifcElementUpdated);
                    }
                }

                updatedFiles.Add(filteredIfcFile);
            }

            return updatedFiles;
        }

        public List<IfcFile> GetWrongLayerMappings(List<IfcFile> filteredIfcFiles, List<LayerMappingItem> layerMappingItems)
        {
            List<IfcFile> updatedFiles = new List<IfcFile>();

            foreach (IfcFile ifcFile in filteredIfcFiles)
            {
                IfcFile updatedFile = new IfcFile
                {
                    IfcElements = new List<IfcElement>(),
                    FilePath = ifcFile.FilePath,
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement updatedElement = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag,
                    };

                    string layer = ifcElement.Layer;

                    LayerMappingItem layerMappingItem = layerMappingItems.FirstOrDefault(item => item.LayerName == layer);

                    if (layerMappingItem == null)
                    {
                        continue;
                    }

                    foreach (KeyValuePair<string, string> keyValuePair in layerMappingItem.PropertiesWithValues)
                    {
                        IfcProperty targetProperty = ifcElement.IfcProperties.FirstOrDefault(item => item.PropertyName == keyValuePair.Key);

                        if (targetProperty == null)
                        {
                            continue;
                        }

                        if (!targetProperty.Value.Equals(keyValuePair.Value))
                        {
                            updatedElement.IfcProperties.Add(targetProperty);
                        }
                    }
                }

                updatedFiles.Add(updatedFile);
            }

            return updatedFiles;
        }

        public List<IfcFile> GetMissingPropertiesData(List<IfcFile> ifcFiles, List<PropertySetItem> propertySetItems)
        {
            List<IfcFile> missingPropertiesFilesData = new List<IfcFile>();

            foreach (IfcFile ifcFile in IfcFiles)
            {
                IfcFile filteredIfcFile = new IfcFile
                {
                    FilePath = ifcFile.FilePath,
                    IfcElements = new List<IfcElement>()
                };

                foreach (IfcElement ifcElement in ifcFile.IfcElements)
                {
                    IfcElement ifcElementUpdated = new IfcElement
                    {
                        Guid = ifcElement.Guid,
                        IfcEntity = ifcElement.IfcEntity,
                        IfcProperties = new List<IfcProperty>(),
                        Layer = ifcElement.Layer,
                        Tag = ifcElement.Tag
                    };

                    bool isAnyPropertyMissed = false;
                    foreach (IfcProperty ifcProperty in ifcElement.IfcProperties)
                    {
                        PropertySetItem propertySetItem = PropertySetItems.FirstOrDefault(propertySetItem => propertySetItem.PropertySetName == ifcProperty.PropertySetName);

                        if (propertySetItem == null)
                        {
                            ifcElementUpdated.IfcProperties.Add(new IfcProperty { PropertySetName = propertySetItem.PropertySetName });
                            isAnyPropertyMissed = true;
                            continue;
                        }

                        PropertyItem propertyItem = propertySetItem.PropertyDefinitions.FirstOrDefault(item => item.PropertyName == ifcProperty.PropertyName);

                        if (propertyItem == null)
                        {
                            ifcElementUpdated.IfcProperties.Add(ifcProperty);
                            isAnyPropertyMissed = true;
                            continue;
                        }
                    }

                    if (isAnyPropertyMissed)
                    {
                        filteredIfcFile.IfcElements.Add(ifcElementUpdated);
                    }
                }

                missingPropertiesFilesData.Add(filteredIfcFile);
            }

            return missingPropertiesFilesData;
        }
    }
}
