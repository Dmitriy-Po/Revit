using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class RoomModel
    {
        public ElementId RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal RoomArea { get; set; }
        
    }
}
