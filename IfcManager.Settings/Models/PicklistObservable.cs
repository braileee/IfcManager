using IfcManager.Settings.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IfcManager.Settings.Models
{

    public class PicklistObservable : BindableBase, INotifyDataErrorInfo
    {

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                ClearErrors(nameof(Name));

                if (Parent?.Picklists.Any(p => p != this && p.Name == value) == true)
                {
                    AddError(nameof(Name), "This picklist name is already used.");
                }

                SetProperty(ref _name, value);
            }
        }



        public ObservableCollection<PicklistItemObservable> Items { get; }
            = new ObservableCollection<PicklistItemObservable>();

        public PicklistViewModel Parent { get; set; }

        public bool HasErrors
        {
            get
            {
                return _errors.Any();
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();


        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            return _errors.ContainsKey(propertyName)
                ? _errors[propertyName]
                : null;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            _errors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

    }
}
