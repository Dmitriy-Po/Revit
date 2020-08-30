using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMTesting.ViewModels
{
    public class SaveChanges : ICommand
    {
        private readonly Action execute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public SaveChanges(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            execute?.Invoke();
        }
    }
}
