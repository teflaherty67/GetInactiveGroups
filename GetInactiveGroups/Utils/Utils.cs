using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetInactiveGroups
{
    internal static class Utils
    {
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static List<string> GetAllGroupsByCategory(Document doc, string categoryValue)
        {
            List<string> groups = new List<string>();

            // Get all sheet views in the project that have the specified category value
            List<ViewSheet> sheets = GetAllSheetsByCategory(doc, categoryValue);

            // Iterate through each sheet view and get the value of the "Group" parameter
            foreach (ViewSheet sheet in sheets)
            {
                // Get the "Group" parameter of the sheet view
                Parameter groupParameter = sheet.LookupParameter("Group");

                // Check if the "Group" parameter is valid and get its value
                if (groupParameter != null && groupParameter.Definition.Name == "Group")
                {
                    string groupValue = groupParameter.AsString();

                    // Check if the group value is not null or empty, and if it hasn't already been added to the list
                    if (!string.IsNullOrEmpty(groupValue) && !groups.Contains(groupValue))
                    {
                        groups.Add(groupValue);
                    }
                }
            }

            return groups;
        }

        public static List<ViewSheet> GetAllSheetsByCategory(Document doc, string categoryValue)
        {
            List<ViewSheet> sheets = new List<ViewSheet>();

            // Get all sheets in the project
            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc);
            ICollection<Element> sheetElements = sheetCollector.OfClass(typeof(ViewSheet)).ToElements();

            // Iterate through each sheet and check if it has the specified category parameter with the value of "Inactive"
            foreach (Element sheetElement in sheetElements)
            {
                ViewSheet sheet = sheetElement as ViewSheet;
                if (sheet != null)
                {
                    // Get the category parameter of the sheet
                    Parameter categoryParameter = sheet.LookupParameter("Category");

                    // Check if the category parameter is valid and has the expected value
                    if (categoryParameter != null && categoryParameter.Definition.Name == "Category" &&
                        categoryParameter.AsValueString() == categoryValue)
                    {
                        sheets.Add(sheet);
                    }
                }
            }

            return sheets;
        }


        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }
    }
}
