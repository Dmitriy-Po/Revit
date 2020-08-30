using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.ReadOnly)]
    public class HelloWorld : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // The first argument, commandData, provides access to the top most object model. 
            // You will get the necessary information from commandData. 
            // To see what's in there, print out a few data accessed from commandData 
            // 
            // Exercise: Place a break point at commandData and drill down the data. 

            UIApplication rvtUiApp = commandData.Application;
            Application rvtApp = rvtUiApp.Application;
            UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument;
            Document rvtDoc = rvtUiDoc.Document;

            // Print out a few information that you can get from commandData 
            string versionName = rvtApp.VersionName;
            string documentTitle = rvtDoc.Title;

            TaskDialog.Show(
              "Revit Intro Lab",
              "Version Name = " + versionName
              + "\nDocument Title = " + documentTitle);

            // Print out a list of wall types available in the current rvt project:

            FilteredElementCollector collector = new FilteredElementCollector(rvtDoc);
            collector.OfClass(typeof(WallType));

            string s = "";
            foreach (WallType wallType in collector)
            {
                s += wallType.Name + " | " + wallType.FamilyName + "\r\n";
            }

            // Show the result:

            TaskDialog.Show(
              "Revit Intro Lab",
              "Wall Types (in main instruction):\n\n" + s);

            // 2nd and 3rd arguments are when the command fails. 
            // 2nd - set a message to the user. 
            // 3rd - set elements to highlight. 

            return Result.Succeeded;
        }
    }
}
