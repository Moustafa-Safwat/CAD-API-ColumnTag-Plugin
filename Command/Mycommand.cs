using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFCADAPI.Command
{
    public class Mycommand : ICommand
    {
        #region Event
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Constructor
        public Mycommand(Action<object> excute, Predicate<object> canexcute)
        {
            this.Excute = excute;
            this.CanExcute = canexcute;
        }
        #endregion

        #region Property
        public Action<object> Excute;
        public Predicate<object> CanExcute;
        #endregion

        #region Methods
        public bool CanExecute(object parameter)
        {
            return CanExcute(parameter);
        }

        public void Execute(object parameter)
        {
            Excute(parameter);
        }
        #endregion

    }
}
