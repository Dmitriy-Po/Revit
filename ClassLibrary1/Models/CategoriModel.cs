using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ClassLibrary1.Models
{
    /// <summary>
    /// Класс для категории объекта в модели. Например для стен, дверей, окон.
    /// </summary>
    public class CategoriModel : INotifyPropertyChanged
    {
        private ElementId id;
        private string nameCategoti;
        private ObservableCollection<UserView> viewws;
        

        private void OnChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
                


        public ElementId IdCategori
        {
            get => id;
            set
            {
                id = value;
                OnChanged(nameof(IdCategori));
            }
        }

        public string NameCategori
        { 
            get { return nameCategoti; }
            set
            {
                nameCategoti = value;
                OnChanged(nameof(NameCategori));
            }
        }
        
        public ObservableCollection<UserView> UsersViews 
        {
            get { return viewws; }
            set
            {
                viewws = value;
                OnChanged(nameof(UsersViews));
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
