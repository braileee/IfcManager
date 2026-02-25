using Autodesk.Revit.DB;
using IfcManager.BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitIfcManager.Models
{
    public class ParametersTagElementsOptions
    {
        public List<PropertyField> ChangedFields { get; set; } = new List<PropertyField>();
        public List<Element> Elements { get; set; } = new List<Element>();
        public List<ExpressionItem> Expressions { get; set; } = new List<ExpressionItem>();
        public ObservableCollection<PropertyField> Fields { get; internal set; } = new ObservableCollection<PropertyField>();
        public List<ComposedPropertyItem> ComposedItems { get; set; } = new List<ComposedPropertyItem>();
        public List<PropertyValueMatch> PropertyValueExactMatches { get; set; } = new List<PropertyValueMatch>();
    }
}
