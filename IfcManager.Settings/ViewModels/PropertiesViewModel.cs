using IfcManager.BL;
using IfcManager.BL.Json;
using IfcManager.BL.Models;
using IfcManager.Settings.Models;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.Settings.ViewModels
{
    public class PropertiesViewModel : BindableBase
    {
        public ObservableCollection<PropertySetItemObservable> PropertySets { get; }
       = new ObservableCollection<PropertySetItemObservable>();

        public DelegateCommand AddPropertySetCommand { get; }
        public DelegateCommand<PropertySetItemObservable> RemovePropertySetCommand { get; }

        public DelegateCommand<PropertySetItemObservable> AddPropertyCommand { get; }
        public DelegateCommand<PropertyItemObservable> RemovePropertyCommand { get; }
        public SettingsRoot SettingsRoot { get; }

        public PropertiesViewModel(SettingsRoot settingsRoot)
        {
            SettingsRoot = settingsRoot;

            AddPropertySetCommand = new DelegateCommand(AddPropertySet);
            RemovePropertySetCommand = new DelegateCommand<PropertySetItemObservable>(RemovePropertySet);

            AddPropertyCommand = new DelegateCommand<PropertySetItemObservable>(AddProperty);
            RemovePropertyCommand = new DelegateCommand<PropertyItemObservable>(RemoveProperty);

            string excelFilePath = ExcelDataLoader.LoadOrPromptExcelFilePath(settingsRoot.ExcelSettings.FileLinkSettings);
            List<PropertySetItem> propertySetItems = ExcelDataLoader.LoadPropertySetItems(excelFilePath, settingsRoot.ExcelSettings.PropertiesSheet);

            foreach (var item in propertySetItems)
            {
                var set = PropertySets.FirstOrDefault(s => s.Name == item.PropertySetName);
                if (set == null)
                {
                    set = new PropertySetItemObservable { Name = item.PropertySetName };
                    PropertySets.Add(set);
                }
                foreach (var prop in item.PropertyItems)
                {
                    set.Properties.Add(new PropertyItemObservable
                    {
                        PropertyName = prop.PropertyName,
                        DataType = Constants.DataTypes.FirstOrDefault(item => item == prop.DataType),
                    });
                }
            }
        }

        private void AddPropertySet()
        {
            PropertySets.Add(new PropertySetItemObservable { Name = "NewSet" });
        }

        private void RemovePropertySet(PropertySetItemObservable set)
        {
            PropertySets.Remove(set);
        }

        private void AddProperty(PropertySetItemObservable set)
        {
            set?.Properties.Add(new PropertyItemObservable { PropertyName = "NewProperty", DataType = Constants.DataTypes.FirstOrDefault() });
        }

        private void RemoveProperty(PropertyItemObservable item)
        {
            foreach (var set in PropertySets)
            {
                if (set.Properties.Contains(item))
                {
                    set.Properties.Remove(item);
                    break;
                }
            }
        }
    }
}
