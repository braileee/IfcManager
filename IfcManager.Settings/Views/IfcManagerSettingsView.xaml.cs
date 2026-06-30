using IfcManager.Settings.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IfcManager.Settings.Views
{
    /// <summary>
    /// Interaction logic for IfcManagerSettingsView.xaml
    /// </summary>
    public partial class IfcManagerSettingsView : Window
    {
        public IfcManagerSettingsView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            IfcManagerSettingsViewModel ifcManagerSettingsViewModel = DataContext as IfcManagerSettingsViewModel;

            if(ifcManagerSettingsViewModel == null)
            {
                return;
            }

            if (ifcManagerSettingsViewModel.PropertiesViewModel.HasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    ifcManagerSettingsViewModel.PropertiesViewModel.SaveCommand.Execute();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
