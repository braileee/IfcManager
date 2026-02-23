using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RevitIfcManager.Models
{
    public class EditorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate ComboTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not PropertyField field)
                return base.SelectTemplate(item, container);

            return field.EditorType switch
            {
                EditorType.Bool => BoolTemplate,
                EditorType.Double => DoubleTemplate,
                EditorType.Int => IntTemplate,
                EditorType.String => StringTemplate,
                EditorType.Combo => ComboTemplate,
                _ => StringTemplate
            };
        }
    }

}
