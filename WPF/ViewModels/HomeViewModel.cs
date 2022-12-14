using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.ViewModels
{
    public class HomeViewModel : MainViewModel
    {
        #region constructor
        #endregion

        #region implementation baseViewModel
        public override bool CanExecute(object parameter)
        {
            switch (parameter.ToString().ToLower())
            {
                case "selectrxdfiles": return true;
                case "selectdbcfile": return true;
            }
            return true;
        }

        public override void Execute(object parameter)
        {
            switch (parameter.ToString().ToLower())
            {
                case "selectrxdfiles": ImportRXDFiles(); break;
                case "selectdbcfile": ImportDBCFile(); break;

                default:
                    break;
            }
        }
        public override string this[string columnName]
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region attributes
        private string _inputPathRXDFile;
        private ObservableCollection<string> _rxdFiles;
        private string _inputPathDBCFile;
        #endregion

        #region properties
        public string InputPathRXDFile
        {
            get { return _inputPathRXDFile; }
            set { _inputPathRXDFile = value; }
        }
        public ObservableCollection<string> RXDFiles
        {
            get { return _rxdFiles; }
            set { _rxdFiles = value; }
        }
        public string InputPathDBCFile
        {
            get { return _inputPathDBCFile; }
            set { _inputPathDBCFile = value; }
        }

        #endregion

        #region basic crud operations
        #endregion

        #region extra methods
        private void ImportRXDFiles()
        {
            RXDFiles = new ObservableCollection<string>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "RXD Files (*.rxd)|*.rxd";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd.ShowDialog() == true)
            {
                foreach (string inputFile in ofd.FileNames)
                {
                    RXDFiles.Add(inputFile);
                }
            }
        }
        private void ImportDBCFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "DBC File (*.dbc)|*.dbc";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd.ShowDialog() == true)
            {
                InputPathDBCFile = ofd.FileName;
            }
        }
        #endregion
    }
}
