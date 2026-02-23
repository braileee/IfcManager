using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSURevitApps.Core
{
    public static class DocumentExtensions
    {
        public static CategorySet GetUsedBoundModelCategories(this Document doc, List<BuiltInCategory> excludedCategories = null)
        {
            var categorySet = new CategorySet();

            // Collect all elements that belong to a category
            var collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .Where(e => e.Category != null && 
                e.Category.CategoryType == CategoryType.Model && 
                e.Category.AllowsBoundParameters);

            foreach (var e in collector)
            {
                if(excludedCategories != null)
                {
                    if (excludedCategories.Contains(e.Category.BuiltInCategory))
                    {
                        continue;
                    }
                }

                categorySet.Insert(e.Category);
            }

            return categorySet;
        }
    }
}
