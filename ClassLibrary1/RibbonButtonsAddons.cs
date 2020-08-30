using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClassLibrary1.ViewModel;


namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    class RibbonButtonsAddons : IExternalCommand
    {
        private static ExternalCommandData externalCommandData;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            externalCommandData = commandData;

            ViewModelRoom viewModel = new ViewModelRoom();
            ExternalEvent externalEvent = ExternalEvent.Create(viewModel);

            // Показать окно.
            ViewRoom u1 = new ViewRoom(externalEvent, commandData, viewModel);
            u1.Show();

            return Result.Succeeded;
        }
        public static ExternalCommandData GetCommandData() => externalCommandData;
    }

    [Transaction(TransactionMode.Manual)]
    class RibbonBtnShowHide : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CategoryForm categoryForm = new CategoryForm();
            VMCategori vM = new VMCategori(commandData.Application.ActiveUIDocument);
            categoryForm.DataContext = vM;
            categoryForm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            categoryForm.Width = 500;
            categoryForm.Height = 500;
            categoryForm.ShowDialog();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class RibbonBtnShowWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;

            #region показать комнаты
            //Document doc = commandData.Application.ActiveUIDocument.Document;
            //UIDocument aui = commandData.Application.ActiveUIDocument;

            //XYZ p1 = aui.Selection.PickPoint();
            //XYZ p2 = aui.Selection.PickPoint();

            //Outline outline = new Outline(p1, p2);
            //BoundingBoxIntersectsFilter bounding = new BoundingBoxIntersectsFilter(outline);

            //FilteredElementCollector elementsWall =
            //    new FilteredElementCollector(doc)
            //    .WherePasses(bounding)
            //    .OfClass(typeof(WallType));


            //string walls = string.Empty;
            //foreach (Element elem in elementsWall)
            //{
            //    walls += elem.Name + Environment.NewLine;
            //}

            //TaskDialog.Show("Стены", walls);
            #endregion

            #region exeption
            //UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            //Document doc = uIDocument.Document;
            //Reference reference;
            //try
            //{
            //    reference = uIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            //}
            //catch (OperationCanceledException oce)
            //{
            //    message = oce.Message;
            //    return Result.Cancelled;
            //}

            //if (reference == null)
            //{
            //    return Result.Cancelled;
            //}

            //Element element = doc.GetElement(reference);
            #endregion

            #region HostObject
            //var hostObjects = new FilteredElementCollector(document)
            //    .OfClass(typeof(HostObject));

            //string information = string.Empty;

            //foreach (HostObject hObject in hostObjects)
            //{
            //    information += hObject.Name + Environment.NewLine;
            //    try
            //    {
            //        information += GetFaceInfo(hObject, HostObjectUtils.GetSideFaces(hObject, ShellLayerType.Exterior));
            //    }
            //    catch (Autodesk.Revit.Exceptions.ArgumentException)
            //    {}
            //    try
            //    {
            //        information += GetFaceInfo(hObject, HostObjectUtils.GetTopFaces(hObject));
            //    }
            //    catch (Autodesk.Revit.Exceptions.ArgumentException)
            //    {}
            //    try
            //    {
            //        information += GetFaceInfo(hObject, HostObjectUtils.GetBottomFaces(hObject));
            //    }
            //    catch (Autodesk.Revit.Exceptions.ArgumentException)
            //    {}
            //}

            //TaskDialog.Show("Faces", information);
            #endregion

            var doors = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Doors).ToElements();
            string catigories = "";

            foreach (Element elem in doors)
            {
                catigories += elem.Category.ToString() + "\n";
            }

            TaskDialog.Show("cat", catigories);
            return Result.Succeeded;
        }

        private string GetFaceInfo(HostObject hObject, IList<Reference> references)
        {
            string info = string.Empty;

            foreach (Reference refr in references)
            {
                Face face = hObject.GetGeometryObjectFromReference(refr) as Face;
                info += face.Area + ", ";

                foreach (EdgeArray edgeArray in face.EdgeLoops)
                {
                    foreach (Edge edge in edgeArray)
                    {
                        info += edge.ApproximateLength + ", ";
                    }
                }

                info += Environment.NewLine;
            }

            return info;
        }
    }

    /// <summary>
    /// Класс для кнопок.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class NewRibon : IExternalApplication
    {
        private PushButtonData CreateButton<B>(string description, string path)
        {
            return new PushButtonData(
                typeof(B).Name,
                description,
                path,
                typeof(B).FullName);
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;        

        public Result OnStartup(UIControlledApplication application)
        {
            string puthFolder = @"C:\Users\Дмитрий\source\repos\ClassLibrary1\ClassLibrary1\bin\Debug";
            string fileDLL = Path.Combine(puthFolder, "ClassLibrary1.dll");
            string nameRibon = "Расширенные команды";

            application.CreateRibbonTab(nameRibon);

            RibbonPanel panelA = application.CreateRibbonPanel(nameRibon, "Панель A");

            PushButton btnShowRooms = (PushButton)panelA.AddItem(
                CreateButton<RibbonButtonsAddons>("Получить список комнат", fileDLL));

            PushButton btnShowAllWalls = (PushButton)panelA.AddItem(
                CreateButton<RibbonBtnShowWall>("Получить список стен", fileDLL));

            PushButton btnShowHide = (PushButton)panelA.AddItem(
                CreateButton<RibbonBtnShowHide>("Показать категории", fileDLL));

            string pathImage = @"C:\Users\Дмитрий\Desktop\Revit\Revit2016APITraining\Labs\2_Revit_UI_API\Images";
            btnShowRooms.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "ImgHelloWorld.png"), UriKind.Absolute));
            btnShowAllWalls.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "Elements.ico"), UriKind.Absolute));
            btnShowHide.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "Parameters.ico"), UriKind.Absolute));

            return Result.Succeeded;
        }
    }
}
