using IfcManager.BL.Json;
using IfcManager.BL.Models;
using IfcManager.Settings.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.Settings.ViewModels
{
    public class PicklistViewModel : BindableBase
    {
        public ObservableCollection<PicklistObservable> Picklists { get; }
            = new ObservableCollection<PicklistObservable>();

        public DelegateCommand<PicklistObservable> AddItemCommand { get; }
        public DelegateCommand<PicklistItemObservable> RemoveItemCommand { get; }
        public SettingsRoot SettingsRoot { get; }
        public DelegateCommand AddPicklistCommand { get; }

        public DelegateCommand<PicklistObservable> RemovePicklistCommand { get; }

        private List<string> availablePicklistGroupNames;
        public List<string> AvailablePicklistGroupNames
        {
            get { return availablePicklistGroupNames; }
            set
            {
                availablePicklistGroupNames = value;
                RaisePropertyChanged();
            }
        }

        public PicklistViewModel(SettingsRoot settingsRoot)
        {
            SettingsRoot = settingsRoot;
            LoadFromExcel();

            AddItemCommand = new DelegateCommand<PicklistObservable>(AddItem);
            RemoveItemCommand = new DelegateCommand<PicklistItemObservable>(RemoveItem);
            SaveCommand = new DelegateCommand(Save);
            AddPicklistCommand = new DelegateCommand(AddPicklist);
            RemovePicklistCommand = new DelegateCommand<PicklistObservable>(OnRemovePicklistCommand);
        }

        private void OnRemovePicklistCommand(PicklistObservable observable)
        {
            if (observable == null) return;

            if (Picklists.Count <= 1)
                return;

            Picklists.Remove(observable);
        }

        private void AddPicklist()
        {
            var newList = new PicklistObservable
            {
                Name = string.Empty,
                Parent = this
            };

            newList.Items.Add(new PicklistItemObservable { Value = "Value1" });

            Picklists.Add(newList);
        }

        private void Save()
        {
            string path = ExcelDataLoader.LoadOrPromptExcelFilePath(SettingsRoot.ExcelSettings.FileLinkSettings);

            List<PicklistGroup> picklists = Picklists.Select(item => new PicklistGroup { GroupName = item.Name, Values = item.Items.Select(item => item.Value).ToList() }).ToList();
            ExcelDataLoader.SavePicklists(
                path,
                SettingsRoot.ExcelSettings.PicklistSheet.SheetName, // add this in settings
                picklists
            );
        }

        public DelegateCommand SaveCommand { get; }
        public List<PropertySetItem> PropertySetItems { get; private set; } = new List<PropertySetItem>();

        private void LoadFromExcel()
        {
            string path = ExcelDataLoader.LoadOrPromptExcelFilePath(SettingsRoot.ExcelSettings.FileLinkSettings);
            PropertySetItems = ExcelDataLoader.LoadPropertySetItems(path, SettingsRoot.ExcelSettings.PropertiesSheet);
            AvailablePicklistGroupNames = PropertySetItems.SelectMany(item => item.PropertyItems).Select(item => item.PropertyName).Distinct().ToList();

            var table = ExcelDataLoader.LoadPicklistGroups(path, SettingsRoot.ExcelSettings.PicklistSheet);

            foreach (var col in table)
            {
                var picklist = new PicklistObservable
                {
                    Name = AvailablePicklistGroupNames.FirstOrDefault(item => item == col.GroupName),
                    Parent = this
                };

                if (string.IsNullOrEmpty(picklist?.Name))
                {
                    continue;
                }

                foreach (var val in col.Values.Where(v => !string.IsNullOrWhiteSpace(v)))
                {
                    picklist.Items.Add(new PicklistItemObservable { Value = val });
                }

                Picklists.Add(picklist);
            }
        }

        private void AddItem(PicklistObservable list)
        {
            list?.Items.Add(new PicklistItemObservable { Value = "NewValue" });
        }

        private void RemoveItem(PicklistItemObservable item)
        {
            foreach (var list in Picklists)
            {
                if (list.Items.Contains(item))
                {
                    list.Items.Remove(item);
                    break;
                }
            }
        }
    }

}
