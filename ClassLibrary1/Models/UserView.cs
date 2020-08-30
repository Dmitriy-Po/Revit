using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace ClassLibrary1.Models
{
    public class UserView : INotifyPropertyChanged
    {
        private string nameUIView;
        private bool status;
        private View view;

        private void OnChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public View View
        {
            get => view;
            set
            {
                view = value;
                OnChanged(nameof(View));
            }
        }

        public string NameUIView 
        {
            get { return nameUIView; }
            set
            {
                nameUIView = value;
                OnChanged(nameof(NameUIView));
            } 
        }
        public bool StatusOfVisible 
        {
            get { return status; }
            set
            {
                status = value;
                OnChanged(nameof(StatusOfVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
