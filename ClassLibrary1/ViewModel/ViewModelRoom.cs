using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClassLibrary1
{
    public class ViewModelRoom : IExternalEventHandler
    {
        DataGrid DataGrid;

        private void SaveNewNames()
        {
            ItemCollection dataRows = DataGrid.Items;
            var rooms = ViewRoom.GetFoundedRooms();

            try
            {
                foreach (RoomModel row in dataRows)
                {
                    rooms.Where(w => w.Id == row.RoomId).FirstOrDefault().Name = row.RoomName;
                }
            }
            catch (Exception)
            {
            }
        }

        public void Execute(UIApplication app)
        {
            using (Transaction trans = new Transaction(app.ActiveUIDocument.Document))
            {
                trans.Start("modyf");
                SaveNewNames();
                trans.Commit();
            }
        }
        
        public void SetDataGrid(DataGrid dataGrid)
        {
            DataGrid = dataGrid;
        }

        public string GetName() => "modyf";
    }
}
