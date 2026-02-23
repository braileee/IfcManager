using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PSURevitApps.Core.Models;
using RevitIfcManager.Models;
using RevitIfcManager.ViewModels;
using RevitIfcManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace RevitIfcManager.EventHandlers
{
    internal class ParametersExcelToRevitEventHandler : RevitEventWrapper<ParametersExcelToRevitOptions>
    {
        public override void Execute(UIApplication app, ParametersExcelToRevitOptions options)
        {
            try
            {
               ProjectParametersGeneration.CreateProjectParameters(app.ActiveUIDocument.Document, options.PropertySetItems);
            }
            catch (Exception exception)
            {
            }
        }
    }
}
