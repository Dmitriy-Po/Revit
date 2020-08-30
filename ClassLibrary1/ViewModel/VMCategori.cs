using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClassLibrary1.Models;
using MVVMTesting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.ViewModel
{
    public class VMCategori : INotifyPropertyChanged
    {
        private UIDocument uIDocument;
        private CategoriModel selectedCategoti;
        private ObservableCollection<CategoriModel> listCaregories;
        private ObservableCollection<UserView> listViews;
        public SaveChanges ApplyChanges { get; private set; }

        private void OnChange(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CategoriModel SelectedCategori
        {
            get => selectedCategoti;
            set 
            {
                selectedCategoti = value;
                OnChange(nameof(SelectedCategori));
            }
        }

        public ObservableCollection<CategoriModel> ListCategory
        {
            get => listCaregories;
            set
            {
                listCaregories = value;
                OnChange(nameof(ListCategory));
            }
        }

        public ObservableCollection<UserView> ListView
        {
            get => listViews;
            set
            {
                listViews = value;
                OnChange(nameof(ListView));
            }
        }

        public VMCategori(UIDocument uIDocument)
        {
            this.uIDocument = uIDocument;
            ListCategory = GetCategories();
            ApplyChanges = new SaveChanges(GoSave);                
        }

        private void GoSave()
        {
            using (Transaction transaction = new Transaction(uIDocument.Document, "ShowHideCategori"))
            {
                transaction.Start();
                Categories categories = uIDocument.Document.Settings.Categories;
                var userViews = ListCategory
                    .Where(w => w.NameCategori == SelectedCategori.NameCategori)
                    .Select(s => s.UsersViews)
                    .FirstOrDefault();

                foreach (UserView view in userViews)
                {
                    Category selectedCategories = 
                        categories.get_Item(SelectedCategori.NameCategori);

                    if (view.View.CanCategoryBeHidden(selectedCategories.Id))
                    {
                        view.View.SetCategoryHidden(selectedCategories.Id, !view.StatusOfVisible);
                    }

                    //categories
                    //    .get_Item(SelectedCategori.NameCategori)
                    //    .set_Visible(view.View, view.StatusOfVisible);
                }
                transaction.Commit();
            }
        }

        public ObservableCollection<CategoriModel> GetCategories()
        {
            Categories cats = uIDocument.Document.Settings.Categories;
            var list = new ObservableCollection<CategoriModel>();
            var views = new FilteredElementCollector(
                uIDocument.Document)
                .OfClass(typeof(View))
                .Cast<View>()
                .ToList();

            // Отбор по некоторым видам: планы, разрезы, фасады.
            var selectedViews = views
                .Where(s => s.ViewType == ViewType.ThreeD
                || s.ViewType == ViewType.Section
                || s.ViewType == ViewType.FloorPlan
                || s.ViewType == ViewType.DrawingSheet);

            foreach (Category category in cats)
            {
                list.Add(
                    new CategoriModel()
                    {
                        NameCategori = category.Name,
                        UsersViews = GetUsersView(views, category),
                        IdCategori = category.Id
                    });                
            }

            return list;
        }

        private ObservableCollection<UserView> GetUsersView(List<View> views, Category category)
        {
            ObservableCollection<UserView> listViews = new ObservableCollection<UserView>();            
            foreach (View view in views)
            {
                listViews.Add(
                    new UserView()
                    {
                        NameUIView = view.Title,
                        StatusOfVisible = category.get_Visible(uIDocument.ActiveView),
                        View = view
                    });
            }

            return listViews;
        }
    }
}
