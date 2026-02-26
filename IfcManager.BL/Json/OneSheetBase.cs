using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Json
{
    public abstract class OneSheetBase : SheetBase
    {
        public string SheetName { get; set; }

        protected OneSheetBase()
        {
            SheetName = string.Empty;
        }
    }
}
