using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitIfcManager.Json
{
    public class SettingsRoot
    {
        public ExcelSettings ExcelSettings { get; set; }
    }
}
