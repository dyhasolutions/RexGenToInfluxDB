using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.ViewModels
{
    public class SFTPViewModel : MainViewModel
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override string this[string columnName]
        {
            get
            {
                return "";
            }
        }
    }
}
