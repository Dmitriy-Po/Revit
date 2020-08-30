using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.ComponentModel;
using System.Windows;

namespace ClassLibrary1
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ViewRoom : Window
    {
        private BindingList<RoomModel> dataBinding;
        private readonly ExternalEvent external;
        private readonly ViewModelRoom modelRoom;


        
        private void GetRooms_Click(object sender, RoutedEventArgs e) => AddRooms();

        private void SetNewNameRoom_Click(object sender, RoutedEventArgs e)
        {
            modelRoom.SetDataGrid(dgRoomsName);
            external.Raise();
        }

        private void AddRooms()
        {
            // Получение данных объекта.
            var foundRooms = GetFoundedRooms();
            dataBinding = new BindingList<RoomModel>();

            // Заполнение.
            foreach (Room room in foundRooms)
            {
                dataBinding.Add(new RoomModel()
                {
                    RoomId = room.Id,
                    // Убрать из наименования идентифакатор помещения.
                    RoomName = room.Name.Remove(room.Name.Length - 2),
                    RoomArea = Math.Round((decimal)room.Area, 3, MidpointRounding.AwayFromZero)
                });
            }
            dgRoomsName.ItemsSource = dataBinding;
        }

        public static FilteredElementCollector GetFoundedRooms()
        {
            UIApplication app = RibbonButtonsAddons.GetCommandData().Application;
            UIDocument uIDocument = app.ActiveUIDocument;

            // Поиск среди элементов, комнат.
            FilteredElementCollector foundRooms =
                new FilteredElementCollector(uIDocument.Document)
                .WherePasses(new RoomFilter());

            return foundRooms;
        }

        public ViewRoom(ExternalEvent externalEvent, ExternalCommandData commandData, ViewModelRoom viewModel)
        {
            InitializeComponent();
            external = externalEvent;
            modelRoom = viewModel;
        }
    }
}
