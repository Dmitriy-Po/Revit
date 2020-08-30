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
    class ElementModification : IExternalCommand
    {
        Application m_rvtApp;
        Document m_rvtDoc;
        const double _mmToFeet = 0.0032808399;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //  Get the access to the top most objects. 
            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;

            m_rvtApp = rvtUIApp.Application;
            m_rvtDoc = rvtUIDoc.Document;

            // (1) pick an object on a screen.
            Reference refPick = rvtUIDoc.Selection.PickObject(ObjectType.Element, "Pick an element");
            Element elem = m_rvtDoc.GetElement(refPick);

            // e.g., we assume we can only modify a wall 
            if (elem is Wall)
            {
                //ModifyElement(m_rvtDoc, elem);
                using (Transaction transaction = new Transaction(m_rvtDoc, "XXXX"))
                {
                    transaction.Start();
                    ModifyElementByTransformUtilsMethods(elem);
                    transaction.Commit();
                }
            }

            #region
            //  find a wall family type with the given name.                
            //Element newWallType = ElementFiltering.FindFamilyType(
            //    m_rvtDoc,
            //    typeof(WallType),
            //    "Базовая стена",
            //    "CW 102-50-100p",
            //    null);

            //  assign a new family type.
            //using (Transaction rt = new Transaction(m_rvtDoc))
            //{
            //    rt.Start("New modofication");

            //    aWall.WallType = (WallType)newWallType;
            //    //aWall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(14.0);
            //    //aWall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Modified by API");
            //    //aWall.get_Parameter(BuiltInParameter.WALL_BASE_HEIGHT_PARAM).Set(1000.0);

            //    rt.Commit();
            //}

            // (2) change its parameters. 
            // as a way of exercise, let's constrain the top of the wall 
            // to the level1 and set an offset. 

            // find the level 1 using the helper function we defined in the lab3. 
            //Level level1 = (Level)ElementFiltering.FindElement(
            //    m_rvtDoc, 
            //    typeof(Level), 
            //    "Level 1", 
            //    null);
            #endregion
            //string msg = "Wall changed: " + "\n" + "\n";
            #region
            //using (Transaction transaction = new Transaction(m_rvtDoc, "Mod trans"))
            //{
            //    transaction.Start();
            //    if (level1 != null)
            //    {
            //        aWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level1.Id);
            //        // Top Constraint 
            //        msg += "Top Constraint to: Level 1" + "\n";
            //    }

            //    // Top Offset Double. hard coding for simplisity here. 
            //    double topOffset = mmToFeet(5000);
            //    aWall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(topOffset);

            //    // Structural Usage = Bearing(1) 
            //    aWall.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_USAGE_PARAM).Set(1);

            //    // Comments - String 
            //    aWall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Modified by API");

            //    transaction.Commit();
            //}

            //msg += "Top Offset to: 5000.0" + "\n";
            //msg += "Structural Usage to: Bearing" + "\n";
            //msg += "Comments added: Modified by API" + "\n";

            //TaskDialog.Show("", msg);

            /*---------------------------------------------------------*/

            // e.g., an element we are given is a door.           
            //FamilyInstance aDoor = (FamilyInstance)elem;

            //  find a door family type with the given name.          
            //Element newDoorType = ElementFiltering.FindFamilyType(
            //    m_rvtDoc,
            //    typeof(FamilySymbol), 
            //    elem.Name,
            //    aDoor.Name,                
            //    BuiltInCategory.OST_Doors);

            ////  assign a new family type
            //aDoor.Symbol = (FamilySymbol)newDoorType;

            //// or use a general way: ChangeTypeId:           
            //aDoor.ChangeTypeId(newDoorType.Id);

            //ModifyElementPropertiesWall(elem);

            //LocationCurve wallLocation = (LocationCurve)aWall.Location;

            // create a new line bound.
            //XYZ newPt1 = new XYZ(0.0, 0.0, 0.0);
            //XYZ newPt2 = new XYZ(20.0, 0.0, 0.0);
            //Line newWallLine = Line.CreateBound(newPt1, newPt2);

            // finally change the curve. 
            //using (Transaction transaction = new Transaction(m_rvtDoc, "Curve"))
            //{
            //    transaction.Start();
            //    wallLocation.Curve = newWallLine;
            //    transaction.Commit();
            //}

            // (3) Optional: change its location, using location curve 
            // LocationCurve also has move and rotation methods. 
            // Note: constaints affect the result. 
            // Effect will be more visible with disjoined wall. 
            // To test this, you may want to draw a single standing wall, 
            // and run this command. 

            //using (Transaction transaction = new Transaction(m_rvtDoc, "Curve"))
            //{
            //    transaction.Start();
            //    LocationCurve wallLocation = (LocationCurve)aWall.Location;

            //    XYZ pt1 = wallLocation.Curve.GetEndPoint(0);
            //    XYZ pt2 = wallLocation.Curve.GetEndPoint(1);

            //    // hard coding the displacement value for simility here. 
            //    double dt = mmToFeet(1000.0);
            //    XYZ newPt1 = new XYZ(pt1.X - dt, pt1.Y - dt, pt1.Z);
            //    XYZ newPt2 = new XYZ(pt2.X - dt, pt2.Y - dt, pt2.Z);

            //    // create a new line bound. 
            //    Line newWallLine = Line.CreateBound(newPt1, newPt2);

            //    // finally change the curve. 
            //    wallLocation.Curve = newWallLine;

            //    // message to the user. 
            //    msg += "Location: start point moved -1000.0 in X-direction" + "\n";
            //    TaskDialog.Show("Revit Intro Lab", msg);
            //    transaction.Commit();
            //}
            #endregion

            //using (Transaction transaction = new Transaction(m_rvtDoc, "XXXX"))
            //{
            //    transaction.Start();
            //    //  move by displacement 
            //    XYZ v = new XYZ(10.0, 10.0, 0.0);
            //    ElementTransformUtils.MoveElement(m_rvtDoc, elem.Id, v);

            //    // try rotate: 15 degree around z-axis. 
            //    XYZ pt1 = XYZ.Zero;
            //    XYZ pt2 = XYZ.BasisZ;
            //    Line axis = Line.CreateBound(pt1, pt2);
            //    ElementTransformUtils.RotateElement(m_rvtDoc, elem.Id, axis, Math.PI / 12.0);
            //    transaction.Commit();
            //}


            return Result.Succeeded;
        }
        public void ModifyElementPropertiesWall(Element elem)
        {
            // Constant to this function. 
            // this is for wall. e.g., "Basic Wall: Exterior - Brick on CMU" 
            // you can modify this to fit your need. 
            const string wallFamilyName = "Basic Wall";
            const string wallTypeName = "Exterior - Brick on CMU";
            const string wallFamilyAndTypeName = wallFamilyName + ": " + wallTypeName;

            // for simplicity, we assume we can only modify a wall 
            if (!(elem is Wall))
            {
                TaskDialog.Show(
                  "Revit Intro Lab",
                  "Sorry, I only know how to modify a wall. Please select a wall.");

                return;
            }
            Wall aWall = (Wall)elem;

            // keep the message to the user.
            string msg = "Wall changed: " + "\n" + "\n";

            // (1) change its family type to a different one. 
            // (You can enhance this to import symbol if you want.)  

            Element newWallType = ElementFiltering.FindFamilyType(
                m_rvtDoc,
                typeof(WallType), 
                wallFamilyName,
                wallTypeName,
                null);

            if (newWallType != null)
            {
                aWall.WallType = (WallType)newWallType;
                msg = msg + "Wall type to: " + wallFamilyAndTypeName + "\n";
            }
        }

        public static double mmToFeet(double mmValue) => mmValue * _mmToFeet;

        private void ModifyElement(Document doc, Element element)
        {
            using (Transaction transaction = new Transaction(doc, nameof(this.ModifyElement)))
            {
                transaction.Start();

                Element newWallType = ElementFiltering.FindFamilyType(
                m_rvtDoc,
                typeof(WallType),
                "Базовая стена",
                "CW 102-50-100p",
                null);

                Wall aWall = (Wall)element;
                aWall.WallType = (WallType)newWallType;

                aWall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(mmToFeet(1000));
                
                /*------------------------------------------------------------------------------*/

                LocationCurve wallLocation = (LocationCurve)aWall.Location;

                // create a new line bound.
                XYZ pt1 = wallLocation.Curve.GetEndPoint(0);
                XYZ pt2 = wallLocation.Curve.GetEndPoint(1);

                double dt = mmToFeet(2000.0);

                XYZ newPt1 = new XYZ(pt1.X - dt, pt1.Y - dt, pt1.Z);
                XYZ newPt2 = new XYZ(pt2.X - dt, pt2.Y - dt, pt2.Z);

                Line newWallLine = Line.CreateBound(newPt1, newPt2);

                // finally change the curve. 
                wallLocation.Curve = newWallLine;

                transaction.Commit();
            }
        }

        public void ModifyElementByTransformUtilsMethods(Element elem)
        {
            // keep the message to the user. 
            string msg = "The element changed: " + "\n\n";

            // try move 
            double dt = mmToFeet(1000.0);
            // hard cording for simplicity. 
            XYZ v = new XYZ(dt, dt, 0.0);

            ElementTransformUtils.MoveElement(m_rvtDoc, elem.Id, v);

            msg = msg + "move by (1000, 1000, 0)" + "\n";


            // try rotate: 15 degree around z-axis. 
            XYZ pt1 = XYZ.Zero;
            XYZ pt2 = XYZ.BasisZ;
            Line axis = Line.CreateBound(pt1, pt2);

            ElementTransformUtils.RotateElement(m_rvtDoc, elem.Id, axis, Math.PI / 12.0);

            msg = msg + "rotate by 15 degree around Z-axis" + "\n";

            // message to the user. 
            TaskDialog.Show("Modify element by utils methods", msg);
        }

    }
}
