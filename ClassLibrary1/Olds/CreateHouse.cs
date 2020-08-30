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
using Autodesk.Revit.DB.Structure;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    class CreateHouse : IExternalCommand
    {
        //  member variables 
        private Application m_rvtApp;
        private Document m_rvtDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //  Get the access to the top most objects.
            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            m_rvtApp = rvtUIApp.Application;
            m_rvtDoc = rvtUIDoc.Document;

            using (Transaction transaction = new Transaction(m_rvtDoc, "Create"))
            {
                transaction.Start();
                List<Wall> walls = CreateWalls();
                AddDoor(walls[0]);
                AddDoor(walls[1]);
                AddDoor(walls[2]);
                AddWindow(walls[3]);
                addRoof(walls);
                transaction.Commit();
            }
            return Result.Succeeded;
        }

        /// <summary>
        /// Создать четыре стены.
        /// </summary>
        /// <returns></returns>
        public List<Wall> CreateWalls()
        {
            // hard coding the size of the house for simplicity 
            double width = ElementModification.mmToFeet(10000.0);
            double depth = ElementModification.mmToFeet(5000.0);

            // get the levels we want to work on. 
            // Note: hard coding for simplicity. Modify here you use 
            // a different template.
            Level level1 = (Level)ElementFiltering.FindElement(m_rvtDoc, typeof(Level), "Level 1", null);

            if (level1 == null)
            {
                TaskDialog.Show("Revit Intro Lab", "Cannot find (Level 1). Maybe you use a different template? Try with DefaultMetric.rte.");
                return null;
            }

            Level level2 = (Level)ElementFiltering.FindElement(m_rvtDoc, typeof(Level), "Level 2", null);

            if (level2 == null)
            {
                TaskDialog.Show("Revit Intro Lab", "Cannot find (Level 2). Maybe you use a different template? Try with DefaultMetric.rte.");
                return null;
            }

            // set four corner of walls. 
            // 5th point is for combenience to loop through.
            double dx = width / 2.0;
            double dy = depth / 2.0;

            List<XYZ> pts = new List<XYZ>()
            {
                new XYZ(-dx, -dy, 0.0),
                new XYZ(dx, -dy, 0.0),
                new XYZ(dx, dy, 0.0),
                new XYZ(-dx, dy, 0.0)
            };

            pts.Add(pts[0]);

            // flag for structural wall or not. 
            bool isStructural = false;

            // save walls we create. 
            List<Wall> walls = new List<Wall>(4);

            // loop through list of points and define four walls. 
            for (int i = 0; i <= 3; i++)
            {
                // define a base curve from two points. 
                Line baseCurve = Line.CreateBound(pts[i], pts[i + 1]);

                // create a wall using the one of overloaded methods. 
                Wall aWall = Wall.Create(m_rvtDoc, baseCurve, level1.Id, isStructural);

                // set the Top Constraint to Level 2 
                aWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);

                // save the wall. 
                walls.Add(aWall);
            }

            // This is important. we need these lines to have shrinkwrap working. 
            m_rvtDoc.Regenerate();
            m_rvtDoc.AutoJoinElements();

            return walls;
        }

        /// <summary>
        /// СОздать вдери
        /// </summary>
        /// <param name="hostWall"></param>
        public void AddDoor(Wall hostWall)
        {
            // hard coding the door type we will use. 
            // e.g., "M_Single-Flush: 0915 x 2134mm
            const string doorFamilyName = "Одиночные-Щитовые";   // "M_Single-Flush" 
            const string doorTypeName = "0915 x 2134 мм";      // "0915 x 2134mm" 
            const string doorFamilyAndTypeName = doorFamilyName + ": " + doorTypeName;

            // get the door type to use. 

            FamilySymbol doorType = (FamilySymbol)ElementFiltering.FindFamilyType(
                m_rvtDoc,
                typeof(FamilySymbol),
                doorFamilyName,
                doorTypeName,
                BuiltInCategory.OST_Doors);

            if (doorType == null)
            {
                TaskDialog.Show($"Revit Intro Lab", "Cannot find (" + doorFamilyAndTypeName + "). " +
                    "Maybe you use a different template? Try with DefaultMetric.rte.");
            }

            // get the start and end points of the wall.
            LocationCurve locCurve = (LocationCurve)hostWall.Location;
            XYZ pt1 = locCurve.Curve.GetEndPoint(0);
            XYZ pt2 = locCurve.Curve.GetEndPoint(1);

            //  calculate the mid point. 
            XYZ pt = (pt1 + pt2) / 2.0;

            // we want to set the reference as a bottom of the wall or level1.
            ElementId idLevel1 = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
            Level level1 = (Level)m_rvtDoc.GetElement(idLevel1);

            // finally, create a door. 

            FamilyInstance aDoor = m_rvtDoc.Create.NewFamilyInstance(pt, doorType, hostWall, level1, StructuralType.NonStructural);
        }

        /// <summary>
        /// Добавление окна.
        /// </summary>
        /// <param name="hostWall"></param>
        public void AddWindow(Wall hostWall)
        {
            // hard coding the window type we will use. 
            // e.g., "M_Fixed: 0915 x 1830mm
            const string windowFamilyName = "Фиксированные"; // "M_Fixed" 
            const string windowTypeName = "0915 x 1830 мм"; // "0915 x 1830mm" 
            const string windowFamilyAndTypeName = windowFamilyName + ": " + windowTypeName;

            double sillHeight = ElementModification.mmToFeet(915);

            // get the door type to use. 
            FamilySymbol windowType =
                (FamilySymbol)ElementFiltering.FindFamilyType(
                 m_rvtDoc,
                 typeof(FamilySymbol),
                 windowFamilyName,
                 windowTypeName,
                 BuiltInCategory.OST_Windows);

            if (windowType == null)
            {
                TaskDialog.Show("Revit Intro Lab", "Cannot find (" +
                    windowFamilyAndTypeName +
                    "). Try with DefaultMetric.rte.");
            }

            // get the start and end points of the wall. 

            LocationCurve locCurve = (LocationCurve)hostWall.Location;
            XYZ pt1 = locCurve.Curve.GetEndPoint(0);
            XYZ pt2 = locCurve.Curve.GetEndPoint(1);

            // calculate the mid point. 
            XYZ pt = (pt1 + pt2) / 2.0;

            // we want to set the reference as a bottom of the wall or level1. 

            ElementId idLevel1 = hostWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
            Level level1 = (Level)m_rvtDoc.GetElement(idLevel1);

            // finally create a window. 
            FamilyInstance aWindow = m_rvtDoc.Create.NewFamilyInstance(pt, windowType, hostWall, level1, StructuralType.NonStructural);

            // set the sill height 
            aWindow.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(sillHeight);
        }

        /// <summary>
        /// Добавление крышы.
        /// </summary>
        /// <param name="walls"></param>
        public void addRoof(List<Wall> walls)
        {
            // hard coding the roof type we will use. 
            // e.g., "Basic Roof: Generic - 400mm" 

            const string roofFamilyName = "Базовая крыша";
            const string roofTypeName = "Типовой - 125мм"; // "Generic - 400mm" 
            const string roofFamilyAndTypeName = roofFamilyName + ": " + roofTypeName;

            // find the roof type
            RoofType roofType = (RoofType)ElementFiltering.FindFamilyType(
                m_rvtDoc, 
                typeof(RoofType), 
                roofFamilyName, 
                roofTypeName, 
                null);

            if (roofType == null)
            {
                TaskDialog.Show("Revit Intro Lab", "Cannot find (" + roofFamilyAndTypeName + "). " +
                    "Maybe you use a different template? Try with DefaultMetric.rte."); ;
            }

            // wall thickness to adjust the footprint of the walls 
            // to the outer most lines. 
            // Note: this may not be the best way. 
            // but we will live with this for this exercise. 

            double wallThickness = walls[0].Width;
            double dt = wallThickness / 2.0;
            List<XYZ> dts = new List<XYZ>(5)
            {
                new XYZ(-dt, -dt, 0.0),
                new XYZ(dt, -dt, 0.0),
                new XYZ(dt, dt, 0.0),
                new XYZ(-dt, dt, 0.0)
            };
            dts.Add(dts[0]);

            // set the profile from four walls 
            CurveArray footPrint = new CurveArray();

            for (int i = 0; i < 4; i++)
            {
                LocationCurve locCurve = (LocationCurve)walls[i].Location;
                XYZ pt1 = locCurve.Curve.GetEndPoint(0) + dts[i];
                XYZ pt2 = locCurve.Curve.GetEndPoint(1) + dts[i + 1];
                Line line = Line.CreateBound(pt1, pt2);
                footPrint.Append(line);
            }

            // get the level2 from the wall 

            ElementId idLevel2 = walls[0].get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
            Level level2 = (Level)m_rvtDoc.GetElement(idLevel2);

            // footprint to morel curve mapping 
            ModelCurveArray mapping = new ModelCurveArray();

            // create a roof.
            FootPrintRoof aRoof = m_rvtDoc.Create.NewFootPrintRoof(footPrint, level2, roofType, out mapping);

            // setting the slope 
            foreach (ModelCurve modelCurve in mapping)
            {
                aRoof.set_DefinesSlope(modelCurve, true);
                aRoof.set_SlopeAngle(modelCurve, 0.5);
            }
        }
    }
}
