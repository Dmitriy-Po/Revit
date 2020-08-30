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
using System.Runtime.Remoting.Messaging;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.ReadOnly)]
    class DBElement : IExternalCommand
    {
        //  Member variables 
        private Application m_rvtApp;
        private Document m_rvtDoc;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            //  Get the access to the top most objects. 
            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;

            m_rvtApp = rvtUIApp.Application;
            m_rvtDoc = rvtUIDoc.Document;

            // (1) pick an object on a screen.
            Reference refPick = rvtUIDoc.Selection.PickObject(ObjectType.Element, "Pick an element");

            // we have picked something. 
            Element elem = m_rvtDoc.GetElement(refPick);

            //' we have picked something. 
            //Element elem = refPick.ElementId;

            // (2) let's see what kind of element we got. 
            ShowBasicElementInfo(elem);

            // (3)
            IdentifyElement(elem);

            // (4) first parameters. 
            ShowParameters(elem, "Element Parameters");

            //  check to see its type parameter as well 
            //ElementId elemTypeId = elem.GetTypeId();
            //ElementType elemType = (ElementType)m_rvtDoc.GetElement(elemTypeId);
            //ShowParameters(elemType, "Type Parameters");

            // access to each parameters.
            //RetrieveParameter(elem, "Element Parameter (by Name and BuiltInParameter)");

            // the same logic applies to the type parameter. 
            //RetrieveParameter(elemType, "Type Parameter (by Name and BuiltInParameter)");

            // (5) location 
            //ShowLocation(elem);

            // (6) geometry - the last piece. (Optional) 
            //ShowGeometry(elem);


            return Result.Succeeded;
        }

        public void ShowBasicElementInfo(Element elem)
        {
            // let's see what kind of element we got. 
            // 
            string s = "You Picked:" + "\n";

            s += " Class name = " + elem.GetType().Name + "\n";
            s += " Category = " + elem.Category.Name + "\n";
            s += " Element id = " + elem.Id.ToString() + "\n" + "\n";

            // and, check its type info. 
            // 
            //Dim elemType As ElementType = elem.ObjectType '' this is obsolete. 
            ElementId elemTypeId = elem.GetTypeId();
            ElementType elemType = (ElementType)m_rvtDoc.GetElement(elemTypeId);

            s += "Its ElementType:" + "\n";
            s += " Class name = " + elemType.GetType().Name + "\n";
            s += " Category = " + elemType.Category.Name + "\n";
            s += " Element type id = " + elemType.Id.ToString() + "\n";

            // finally show it. 

            TaskDialog.Show("Basic Element Info", s);
        }

        public void IdentifyElement(Element elem)
        {

            // An instance of a system family has a designated class. 
            // You can use it identify the type of element. 
            // e.g., walls, floors, roofs. 
            // 
            string s = "";

            if (elem is Wall)
            {
                s = "Wall";
            }
            else if (elem is Floor)
            {
                s = "Floor";
            }
            else if (elem is RoofBase)
            {
                s = "Roof";
            }
            else if (elem is FamilyInstance)
            {
                // An instance of a component family is all FamilyInstance. 
                // We'll need to further check its category. 
                // e.g., Doors, Windows, Furnitures. 
                if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
                {
                    s = "Door";
                }
                else if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows)
                {
                    s = "Window";
                }
                else if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Furniture)
                {
                    s = "Furniture";
                }
                else
                {
                    // e.g. Plant 
                    s = "Component family instance";
                }
            }
            else if (elem is HostObject)
            {
                // check the base class. e.g., CeilingAndFloor. 
                s = "System family instance";
            }
            else
            {
                s = "Other";
            }

            s = "You have picked: " + s;

            // show it. 
            TaskDialog.Show("Identify Element", s);
        }

        // show all the parameter values of the element 
        public void ShowParameters(Element elem, string header)
        {
            IList<Parameter> paramSet = elem.GetOrderedParameters();
            string s = string.Empty;

            foreach (Parameter param in paramSet)
            {
                string name = param.Definition.Name;

                // see the helper function below 
                string val = ParameterToString(param);

                s += name + " = " + val + "\n";
            }


            TaskDialog.Show(header, s);
        }

        // Helper function: return a string from of the given parameter. 
        public static string ParameterToString(Parameter param)
        {
            string val = "none";

            if (param == null)
            {
                return val;
            }

            // to get to the parameter value, we need to pause it depending
            // on its strage type 
            switch (param.StorageType)
            {
                case StorageType.Double:
                    double dVal = param.AsDouble();
                    val = dVal.ToString();
                    break;

                case StorageType.Integer:
                    int iVal = param.AsInteger();
                    val = iVal.ToString();
                    break;

                case StorageType.String:
                    string sVal = param.AsString();
                    val = sVal;
                    break;

                case StorageType.ElementId:
                    ElementId idVal = param.AsElementId();
                    val = idVal.IntegerValue.ToString();
                    break;

                case StorageType.None:
                    break;

                default:
                    break;
            }

            return val;
        }
        
        
        //  example of retrieving a specific parameter indivisually. 
        //  (hard coding for simplicity. This function works best 
        //  with walls and doors.)
        public void RetrieveParameter(Element elem, string header)
        {
            string s = string.Empty;

            // as an experiment, let's pick up some arbitrary parameters. 
            // comments - most of instance has this parameter 

            // (1) by BuiltInParameter. 
            Parameter param =
             elem.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            if (param != null)
            {
                s += "Comments (by BuiltInParameter) = " +
                ParameterToString(param) + "\n";
            }

            // (2) by name. (Mark - most of instance has this parameter.) 
            // if you use this method, it will language specific. 
            param = elem.LookupParameter("Mark");
            if (param != null)
            {
                s += "Mark (by Name) = " + ParameterToString(param) + "\n";
            }

            // the following should be in most of type parameter 
            // 
            param = elem.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);
            if (param != null)
            {
                s += "Type Comments (by BuiltInParameter) = " +
                ParameterToString(param) + "\n";
            }

            param = elem.LookupParameter("Fire Rating");
            if (param != null)
            {
                s += "Fire Rating (by Name) = " + ParameterToString(param) +
                    "\n";
            }

            // using the BuiltInParameter, you can sometimes access one that is
            // not in the parameters set. 
            // Note: this works only for element type. 

            param = elem.get_Parameter(
             BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);
            if (param != null)
            {
                s += "SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM (only by BuiltInParameter) = " +
                 ParameterToString(param) + "\n";
            }

            param = elem.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM);
            if (param != null)
            {
                s += "SYMBOL_FAMILY_NAME_PARAM (only by BuiltInParameter) = "
                    + ParameterToString(param) + "\n";
            }

            // show it. 

            TaskDialog.Show(header, s);
        }

        // show the location information of the given element. 
        // location can be LocationPoint (e.g., furniture), and LocationCurve
        // (e.g., wall).
        public void ShowLocation(Element elem)
        {
            string s = "Location Information: " + "\n" + "\n";
            Location loc = elem.Location;

            if (loc is LocationPoint)
            {
                // (1) we have a location point 

                LocationPoint locPoint = (LocationPoint)loc;
                XYZ pt = locPoint.Point;
                double r = locPoint.Rotation;

                s += "LocationPoint" + "\n";
                s += "Point = " + PointToString(pt) + "\n";
                s += "Rotation = " + r.ToString() + "\n";
            }
            else if (loc is LocationCurve)
            {
                // (2) we have a location curve 

                LocationCurve locCurve = (LocationCurve)loc;
                Curve crv = locCurve.Curve;

                s += "LocationCurve" + "\n";
                s += "EndPoint(0)/Start Point = " +
                 PointToString(crv.GetEndPoint(0)) + "\n";
                s += "EndPoint(1)/End point = " +
                 PointToString(crv.GetEndPoint(1)) + "\n";
                s += "Length = " + crv.Length.ToString() + "\n";

                // Location Curve also has property JoinType at the end 

                s += "JoinType(0) = " + locCurve.get_JoinType(0).ToString() + "\n";
                s += "JoinType(1) = " + locCurve.get_JoinType(1).ToString() + "\n";

            }

            // show it. 

            TaskDialog.Show("Show Location", s);
        }

        // Helper Function: returns XYZ in a string form. 
        public static string PointToString(XYZ pt)
        {
            if (pt == null)
            {
                return "";
            }

            return string.Format("({0},{1},{2})",
                pt.X.ToString("F2"), pt.Y.ToString("F2"), pt.Z.ToString("F2"));
        }


        // show the geometry information of the given element.
        public void ShowGeometry(Element elem)
        {
            // Set a geometry option 
            Options opt = m_rvtApp.Create.NewGeometryOptions();
            opt.DetailLevel = ViewDetailLevel.Fine;

            // Get the geometry from the element 
            GeometryElement geomElem = elem.get_Geometry(opt);

            // If there is a geometry data, retrieve it as a string to show it.  
            string s = (geomElem == null) 
                ? "no data" 
                : GeometryElementToString(geomElem);

            TaskDialog.Show("Show Geometry", s);
        }


        // Helper Function: parse the geometry element by geometry type.
        // Here we look at the top level. 
        public static string GeometryElementToString(GeometryElement geomElem)
        {
            string str = string.Empty;

            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomObj is Solid)
                {
                    // ex. wall 

                    Solid solid = (Solid)geomObj;
                    //str += GeometrySolidToString(solid) 

                    str += "Solid" + "\n";
                }
                else if (geomObj is GeometryInstance)
                {
                    // ex. door/window 

                    str += " -- Geometry.Instance -- " + "\n";
                    GeometryInstance geomInstance = (GeometryInstance)geomObj;
                    GeometryElement geoElem = geomInstance.SymbolGeometry;

                    str += GeometryElementToString(geoElem);
                }
                else if (geomObj is Curve)
                {
                    Curve curv = (Curve)geomObj;
                    //str += GeometryCurveToString(curv) 

                    str += "Curve" + "\n";
                }
                else if (geomObj is Mesh)
                {
                    Mesh mesh = (Mesh)geomObj;
                    //str += GeometryMeshToString(mesh) 

                    str += "Mesh" + "\n";
                }
                else
                {
                    str += " *** unkown geometry type" +
                         geomObj.GetType().Name;
                }
            }

            return str;
        }
    }
}
