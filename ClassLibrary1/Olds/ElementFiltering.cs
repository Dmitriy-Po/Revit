using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    class ElementFiltering : IExternalCommand
    {
        //  member variables 
        private Application RevitApp;
        private Document RevitDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //  Get the access to the top most objects. 
            UIApplication revitUIApp = commandData.Application;
            UIDocument revitUIDoc = revitUIApp.ActiveUIDocument;

            RevitApp = revitUIApp.Application;
            RevitDoc = revitUIDoc.Document;

            // Call the main funtion.
            ListFamilyTypes(commandData);

            return Result.Succeeded;
        }
        public void ListFamilyTypes(ExternalCommandData commandData)
        {
            // 
            // (1) get a list of family types available in the current rvt project. 

            // (1.1a) here is an example with wall type. 

            //var wallTypeCollector1 = new FilteredElementCollector(RevitDoc);
            //wallTypeCollector1.WherePasses(new ElementClassFilter(typeof(WallType)));
            //IList<Element> wallTypes1 = wallTypeCollector1.ToElements();


            // using a helper funtion to display the result here. See code below.
            //ShowElementList(wallTypes1, "Wall Types (by Filter): ");

            // (1.1b) the following are the same as two lines above. 
            // these alternative forms are provided for convenience. 
            // using OfClass() 
            // 
            //FilteredElementCollector wallTypeCollector2 = new FilteredElementCollector(RevitDoc);
            //wallTypeCollector2.OfClass(typeof(WallType));
            //ShowElementList(wallTypeCollector2.ToElements(), "Wall Types (by Filter): ");

            // (1.1c) the following are the same as above. For convenience. 
            // using short cut this time. 
            // 
            //FilteredElementCollector wallTypeCollector3 =
            //    new FilteredElementCollector(RevitDoc)
            //    .OfClass(typeof(WallType));
            
            //ShowElementList(wallTypeCollector3.ToElements(), "Типы стен: ");


            // 
            // (2) Listing for component family types. 
            // 
            // For component family, it is slightly different. 
            // There is no designate property in the document class. 
            // You always need to use a filtering. e.g., for doors and windows. 
            // Remember for component family, you will need to check 
            // element type and category.  

            //var doorTypeCollector = new FilteredElementCollector(RevitDoc);
            //doorTypeCollector.OfClass(typeof(FamilySymbol));
            //doorTypeCollector.OfCategory(BuiltInCategory.OST_Doors);
            //IList<Element> doorTypes = doorTypeCollector.ToElements();


            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            Reference refPick = rvtUIDoc.Selection.PickObject(ObjectType.Element, "Pick an element");
            Element elem = rvtUIDoc.Document.GetElement(refPick);



            //List<Element> el = new List<Element>
            //{
            //    FindFamilyType_Wall_v1("", elem.Name)
            //};

            //ShowElementList(el, "Пустой заголовок : ");
            //ShowElementList(doorTypes, "Door Types (by Filter): ");

            //WallType wt = elem as WallType; null

            ElementType wallType3 = (ElementType)FindFamilyType(
                RevitDoc,
                typeof(WallType),
                elem.Name,
                elem.GetType().ToString(),
                null);

            //ElementType doorType3 = (ElementType)FindFamilyType(
            //    RevitDoc, 
            //    typeof(FamilySymbol),
            //    "Одиночные-Щитовые", 
            //    "0915 x 2134 мм",
            //    BuiltInCategory.OST_Doors);
           
            ShowElementList(new List<Element> { wallType3 }, "");
        }

        // Helper function to display info from a list of elements passed onto. 

        public void ShowElementList(IList<Element> elems, string header)
        {
            string s = " - Class - Category - Name (or Family: Type Name) - Id - \r\n";
            foreach (Element e in elems)
            {
                s += ElementToString(e);
            }

            TaskDialog.Show(header + "(" + elems.Count.ToString() + "):", s);
        }

        // Helper function: summarize an element information as a line of text, 
        // which is composed of: class, category, name and id. 
        // name will be "Family: Type" if a given element is ElementType. 
        // Intended for quick viewing of list of element, for example.

        public string ElementToString(Element e)
        {
            if (e == null)
            {
                return "none";
            }

            string name = "";

            if (e is ElementType)
            {
                Parameter param = e.get_Parameter(
                    BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);
                if (param != null)
                {
                    name = param.AsString();
                }
            }
            else
            {
                name = e.Name;
            }

            return e.GetType().Name + "; "
              + e.Category.Name + "; "
              + name + "; "
              + e.Id.IntegerValue.ToString() + "\r\n";
        }
        public Element FindFamilyType_Wall_v1(string wallFamilyName, string wallTypeName)
        {
            // narrow down a collector with class.
            var wallTypeCollector1 = new FilteredElementCollector(RevitDoc);
            wallTypeCollector1.OfClass(typeof(WallType));

            // LINQ query 
            var wallTypeElems1 = wallTypeCollector1
                .Where(w => w.Equals(wallTypeName));

            // get the result.
            return wallTypeElems1.Count() > 0
                ? wallTypeElems1.First()
                : null;        
        }

        //  find an element of the given type, name, and category(optional) 

        public static Element FindFamilyType(
            Document rvtDoc, 
            Type targetType,
            string targetFamilyName,
            string targetTypeName,
            BuiltInCategory? targetCategory)
        {
            // first, narrow down to the elements of the given type and category
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);

            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // parse the collection for the given names 
            // using LINQ query here.
            //var result = collector.Where(
            //    w => w.Name == targetTypeName && 
            //    w.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM)
            //        .AsString()
            //        .Equals(targetFamilyName));

            var targetElems =
                from element in collector
                where element.Name.Equals(targetTypeName) &&
                element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).
                AsString().Equals(targetFamilyName)
                select element;

            // put the result as a list of element fo accessibility.
            IList<Element> elems = targetElems.ToList();

            // return the result. 
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }

        public static Element FindElement(
            Document rvtDoc, 
            Type targetType,
            string targetName,
            BuiltInCategory? targetCategory)
        {
            // find a list of elements using the overloaded method. 
            IList<Element> elems = FindElements(rvtDoc, targetType, targetName, targetCategory);

            // return the first one from the result. 
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }

        public static IList<Element> FindElements(
        Document rvtDoc,
        Type targetType,
        string targetName,
        BuiltInCategory? targetCategory)
        {
            // first, narrow down to the elements of the given type and category 
            var collector =
                new FilteredElementCollector(rvtDoc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // parse the collection for the given names 
            // using LINQ query here.

            var elems =
                from element in collector
                where element.Name.Equals(targetName)
                select element;

            // put the result as a list of element for accessibility. 

            return elems.ToList();
        }
    }
}
