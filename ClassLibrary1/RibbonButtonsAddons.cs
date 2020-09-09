﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using ClassLibrary1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;


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

    [Transaction(TransactionMode.Manual)]
    class RibbonBtnSetNewColorRoom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document DOC = commandData.Application.ActiveUIDocument.Document;
            var allRooms = new FilteredElementCollector(DOC)
                   .WherePasses(new RoomFilter())
                   .Where(w => w.LookupParameter("ROM_Зона").AsString().Contains("Квартира"))
                   .ToList()
                      .GroupBy
                      (
                          g => new
                          {
                              Уровнеь = g.LevelId,
                              BS_Блок = g.GetParameters("BS_Блок").FirstOrDefault().AsString(),
                              ROM_Подзона = g.GetParameters("ROM_Подзона").FirstOrDefault().AsString(),
                              ROM_Зона = g.GetParameters("ROM_Зона").FirstOrDefault().AsString()
                          }
                      )
                   .OrderBy(o => o.Key.Уровнеь.IntegerValue)
                   .ThenBy(t => t.Key.BS_Блок)
                   .ThenBy(t => t.Key.ROM_Подзона)
                   .ThenBy(t => t.Key.ROM_Зона);

            string previorsRoom = "";

            using (Transaction transaction = new Transaction(DOC, "Set new Values"))
            {
                transaction.Start();
                bool ПредыдущаяКомнатаБылаРаскрашена = false;

                foreach (var groupOfRooms in allRooms)
                {
                    if (previorsRoom != "")
                    {
                        // Если текущий элемент, по значению ROM_Зона совпадает с предыдущим, то это значит, что комнаты имеют сквозную нумерацию.
                        // Уточнение: условие выполнится если, значение текущего элемента по параметру ROM_Зона уменьшенного на единицу, совпадает с предыдущим элементом.
                        // Пример: текущий_элемент.Квартира (07 -1) == предыдущий_элемент.Квартира (06).					
                        if ((ROMStringToInt(groupOfRooms.Key.ROM_Зона)-1 == ROMStringToInt(previorsRoom))
                            && !ПредыдущаяКомнатаБылаРаскрашена)
                        {
                            SetNewValues(groupOfRooms);
                            ПредыдущаяКомнатаБылаРаскрашена = true;
                        }
                        else
                        {
                            ПредыдущаяКомнатаБылаРаскрашена = false;
                        }
                    }
                    previorsRoom = groupOfRooms.Key.ROM_Зона;
                }
                transaction.Commit();
            }

            return Result.Succeeded;
        }


        private void SetNewValues(IGrouping<object, Element> groupOfRooms)
        {
            foreach (var room in groupOfRooms)
            {
                Parameter ROM_index = room.LookupParameter("ROM_Подзона_Index");
                string ROM_under_index = room.GetParameters("ROM_Подзона_ID").FirstOrDefault().AsString();                

                // Присовить формат будущего значения, например: "2к.Полутон".
                // TODO: добавить проверку данного, если такое значение уже существует.
                ROM_index.Set(ROM_under_index + ".Полутон");
            }
        }

        /// <summary>
        /// Функция для преобразования строкового представления в целое.
        /// </summary>
        /// <param name="nameRoom">Наименование квартиры.</param>
        /// <returns>Целое число - номер квартиры.</returns>
		private int ROMStringToInt(string nameRoom)
        {
            // Выбрать первое попавшееся совпадение - номер квартиры.
            string numberRoom = new Regex(@"\d+", RegexOptions.IgnoreCase).Match(nameRoom).Value;

            return int.Parse(numberRoom);
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

            PushButton btnNewColor = (PushButton)panelA.AddItem(
                CreateButton<RibbonBtnSetNewColorRoom>("Выделить подходящие квартиры", fileDLL));

            string pathImage = @"C:\Users\Дмитрий\Desktop\Revit\Revit2016APITraining\Labs\2_Revit_UI_API\Images";
            btnShowRooms.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "ImgHelloWorld.png"), UriKind.Absolute));
            btnShowAllWalls.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "Elements.ico"), UriKind.Absolute));
            btnShowHide.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "Parameters.ico"), UriKind.Absolute));
            btnNewColor.LargeImage = new BitmapImage(new Uri(Path.Combine(pathImage, "eye.png")));


            return Result.Succeeded;
        }
    }
}
