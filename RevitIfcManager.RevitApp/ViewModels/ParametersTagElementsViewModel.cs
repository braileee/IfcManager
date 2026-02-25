using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using IfcManager.BL.Json;
using IfcManager.BL.Models;
using NPOI.Util.Collections;
using Prism.Commands;
using Prism.Mvvm;
using PSURevitApps.Core;
using RevitIfcManager.EventHandlers;
using RevitIfcManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using static NPOI.HSSF.UserModel.HeaderFooter;

namespace RevitIfcManager.ViewModels
{
    public class ParametersTagElementsViewModel : BindableBase
    {
        private List<Element> selectedElements;

        public ParametersTagElementsViewModel(UIApplication uiapp)
        {
            Uiapp = uiapp;

            // Load settings
            SettingsRoot settingsRoot = SettingsLoader.LoadExistingOrDefault();

            if (settingsRoot == null)
            {
                MessageBox.Show($"No settings file found", "Error");
                return;
            }

            string excelFilePath = ExcelDataLoader.LoadOrPromptExcelFilePath();

            List<PropertySetItem> propertySetItems = ExcelDataLoader.LoadPropertySetItems(excelFilePath, settingsRoot.ExcelSettings);
            List<PicklistGroup> picklistGroups = ExcelDataLoader.LoadPicklistGroups(excelFilePath, settingsRoot.ExcelSettings);
            PropertyValueMatches = ExcelDataLoader.LoadPropertiesValueMatches(excelFilePath, settingsRoot.ExcelSettings);
            Expressions = ExcelDataLoader.LoadExpressions(excelFilePath, settingsRoot.ExcelSettings);
            ComposedItems = ExcelDataLoader.LoadComposed(excelFilePath, settingsRoot.ExcelSettings);
            PropertyValueExactMatches = ExcelDataLoader.LoadPropertiesExactValueMatches(excelFilePath, settingsRoot.ExcelSettings);

            CustomProperties = propertySetItems.SelectMany(item => item.PropertyItems).ToList();
            Dictionary<string, List<string>> propertiesWithValues = picklistGroups.ToDictionary(item => item.GroupName, item => item.Values);

            PropertiesWithValues = propertiesWithValues;

            LoadFields();

            uiapp.SelectionChanged -= SelectionChanged;
            uiapp.SelectionChanged += SelectionChanged;

            ApplyCommand = new DelegateCommand(OnApplyCommand);

            ParametersTagElementsEventHandler = new ParametersTagElementsEventHandler();
        }

        private void FieldsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (PropertyField item in e.NewItems)
                    item.PropertyChanged += ItemPropertyChanged;

            if (e.OldItems != null)
                foreach (PropertyField item in e.OldItems)
                    item.PropertyChanged -= ItemPropertyChanged;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyField changedField = sender as PropertyField;

            if (changedField == null)
            {
                return;
            }

            List<PropertyValueMatch> sourceMatches = PropertyValueMatches.Where(item => item.PropertyNameSource == changedField.Name).ToList();

            if (sourceMatches.Count == 0)
            {
                return;
            }

            List<string> targetPropertyNames = sourceMatches.Select(item => item.PropertyNameTarget).Distinct().ToList();
            List<string> sourcePropertyNames = sourceMatches.Select(item => item.PropertyNameSource).Distinct().ToList();

            foreach (PropertyField field in Fields)
            {
                if (field.LookupValues == null || field.Name == changedField.Name)
                {
                    continue;
                }

                if (field.LookupValues.Count == 0)
                {
                    field.LookupValues.AddRange(field.SourceLookupValues);
                    continue;
                }

                if (!targetPropertyNames.Contains(field.Name))
                {
                    continue;
                }

                List<PropertyValueMatch> targetMatches = sourceMatches.Where(item => item.PropertyNameTarget == field.Name).ToList();
                List<PropertyValueMatch> sourceMatchValueItems = sourceMatches.Where(item => item.PropertyValueSource == changedField.Value?.ToString()).ToList();
                List<string> targetMatchesValues = sourceMatchValueItems.Select(item => item.PropertyValueTarget).ToList();

                List<string> lookupValues = targetMatchesValues.Count > 0 ? targetMatchesValues : field.SourceLookupValues.ToList();

                if (lookupValues.Count > 0)
                {
                    field.LookupValues.Clear();
                    field.LookupValues.AddRange(lookupValues);
                }
            }

            if (Fields == null || Fields.Count == 0)
            {
                LoadFields();
            }
        }

        private void LoadFields()
        {
            Fields = new ObservableCollection<PropertyField>();
            var newFields = new ObservableCollection<PropertyField>(
                            CustomProperties.Select(s =>
                                new PropertyField(
                                    s.PropertyName,
                                    s.DataType,
                                    PropertiesWithValues.TryGetValue(s.PropertyName, out var values)
                                        ? values
                                        : null
                                )
                                )
                             );

            foreach (var newField in newFields)
            {
                Fields.Add(newField);
            }

            List<string> expressionParameters = Expressions.Select(item => item.TargetPropertyName).Distinct().ToList();
            ApplyReadOnly(expressionParameters);

            List<string> composedParameters = ComposedItems.Select(item => item.ComposedPropertyName).Distinct().ToList();
            ApplyReadOnly(composedParameters);

            List<string> propertyValueExactNames = PropertyValueExactMatches.Select(item => item.PropertyNameTarget).Distinct().ToList();
            ApplyReadOnly(propertyValueExactNames);
        }

        private void ApplyReadOnly(List<string> expressionParameters)
        {
            foreach (var field in Fields)
            {
                if (expressionParameters.Contains(field.Name))
                {
                    field.CanBeEdited = false;
                    field.IsReadOnly = true;
                }
            }
        }

        private void OnApplyCommand()
        {
            List<PropertyField> changedFields = Fields.Where(f => f.Changed).ToList();

            ParametersTagElementsEventHandler.Raise(new ParametersTagElementsOptions
            {
                Elements = SelectedElements,
                ChangedFields = changedFields,
                Fields = Fields,
                Expressions = Expressions,
                ComposedItems = ComposedItems,
                PropertyValueExactMatches = PropertyValueExactMatches
            });
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Fields == null || Fields.Count == 0)
            {
                LoadFields();
            }

            string variesValue = "***VARIES***";

            if(Uiapp.ActiveUIDocument == null)
            {
                return;
            }

            var selectedIds = Uiapp.ActiveUIDocument.Selection.GetElementIds();

            SelectedElementsInfo = $"Elements selected: {selectedIds.Count}";

            if (selectedIds.Count == 0)
            {
                SelectedElements = new List<Element>();
                foreach (var field in fields)
                {
                    field.Value = null;
                }
                return;
            }

            SelectedElements = selectedIds.Select(id => Uiapp.ActiveUIDocument.Document.GetElement(id)).ToList();

            foreach (PropertyField propertyField in Fields)
            {
                EditorType editorType = propertyField.EditorType;
                Element firstElement = SelectedElements.FirstOrDefault();

                string propertyName = propertyField.Name;

                object firstElementValue = firstElement?.LookupParameter(propertyField.Name)?.GetValueAsObject();
                object firstElementValueString = firstElementValue?.ToString() ?? string.Empty;

                bool allValuesSame = SelectedElements.Where(item => item.LookupParameter(propertyField.Name) != null).All(element =>
                {
                    string currentStringValue = element.LookupParameter(propertyField.Name)?.GetValueAsObject()?.ToString();
                    currentStringValue = currentStringValue ?? string.Empty;

                    return currentStringValue.Equals(firstElementValueString);
                });

                if (allValuesSame)
                {
                    if (propertyField.EditorType == EditorType.Bool)
                    {
                        propertyField.Value = firstElementValue?.ToString() == "1" ? true : false;
                    }
                    else
                    {
                        propertyField.Value = firstElementValue;
                    }

                    if(propertyField.EditorType == EditorType.Combo)
                    {
                        if (propertyField.LookupValues.Contains(variesValue))
                        {
                            propertyField.LookupValues.Remove(variesValue);
                        }
                    }
                }
                else
                {
                    if (propertyField.EditorType == EditorType.Combo)
                    {
                        if (!propertyField.LookupValues.Contains(variesValue))
                        {
                            propertyField.LookupValues.Add(variesValue);
                        }
                    }

                    propertyField.Value = variesValue;
                }

                propertyField.Changed = false;
            }
        }

        public UIApplication Uiapp { get; }
        public List<PropertyValueMatch> PropertyValueMatches { get; } = new List<PropertyValueMatch>();
        public List<ExpressionItem> Expressions { get; private set; } = new List<ExpressionItem>();
        public List<ComposedPropertyItem> ComposedItems { get; } = new List<ComposedPropertyItem>();
        public List<PropertyValueMatch> PropertyValueExactMatches { get; } = new List<PropertyValueMatch>();
        public List<PropertyItem> CustomProperties { get; } = new List<PropertyItem>();
        public Dictionary<string, List<string>> PropertiesWithValues { get; } = new Dictionary<string, List<string>>();
        public ObservableCollection<PropertyField> Fields
        {
            get
            {
                return fields;
            }

            set
            {
                if (fields != null)
                {
                    Fields.CollectionChanged -= FieldsCollectionChanged;
                }

                fields = value;

                if (fields != null)
                {
                    Fields.CollectionChanged += FieldsCollectionChanged;
                }

                RaisePropertyChanged();
            }
        }
        public DelegateCommand ApplyCommand { get; }
        public ParametersTagElementsEventHandler ParametersTagElementsEventHandler { get; }

        public List<Element> SelectedElements
        {
            get
            {
                return selectedElements;
            }

            set
            {
                selectedElements = value;
                RaisePropertyChanged();
            }
        }

        private string selectedElementsInfo;
        private ObservableCollection<PropertyField> fields = new ObservableCollection<PropertyField>();

        public string SelectedElementsInfo
        {
            get { return selectedElementsInfo; }
            set
            {
                selectedElementsInfo = value;
                RaisePropertyChanged();
            }
        }

    }
}
