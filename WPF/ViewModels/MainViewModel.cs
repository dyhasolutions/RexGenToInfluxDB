using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF.Views;

namespace WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region constructor

        public MainViewModel()
        {
            //Theme = "B";
            //IconTheme = "MoonWaningCrescent";
        }

        #endregion constructor

        #region implementation baseViewModel

        public override void Execute(object parameter)
        {
            MainView activeWindow = (MainView)Application.Current.Windows.OfType
            <System.Windows.Window>().SingleOrDefault(x => x.IsActive);

            switch (parameter.ToString().ToLower())
            {
                case "rxd":
                    homeViewModel = new HomeViewModel();
                    activeWindow.mainPanel.Children.Clear();
                    Home homeView = new Home
                    {
                        DataContext = homeViewModel
                    };
                    activeWindow.mainPanel.Children.Add(homeView);
                    break;

                case "sftp":
                    sftpViewModel = new SFTPViewModel();
                    activeWindow.mainPanel.Children.Clear();
                    SFTP sftpView = new SFTP
                    {
                        DataContext = sftpViewModel
                    };
                    activeWindow.mainPanel.Children.Add(sftpView);
                    break;

                case "aws":
                    awsViewModel = new AWSViewModel();
                    activeWindow.mainPanel.Children.Clear();
                    AWS awsView = new AWS
                    {
                        DataContext = awsViewModel
                    };
                    activeWindow.mainPanel.Children.Add(awsView);
                    break;

                case "settings":
                    settingsViewModel = new SettingsViewModel();
                    activeWindow.mainPanel.Children.Clear();
                    Settings settingsView = new Settings
                    {
                        DataContext = settingsViewModel
                    };
                    activeWindow.mainPanel.Children.Add(settingsView);
                    break;

                case "theme":
                    ChangeTheme();
                    break;

                case "minimize":
                    activeWindow.WindowState = WindowState.Minimized;
                    break;

                case "close":
                    activeWindow.Close();
                    break;

                default:
                    break;
            }
        }

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

        #endregion implementation baseViewModel

        #region attributes

        private HomeViewModel homeViewModel;
        private SFTPViewModel sftpViewModel;
        private AWSViewModel awsViewModel;
        private SettingsViewModel settingsViewModel;
        private string _title;

        #endregion attributes

        #region properties

        public string title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        public string Theme { get; set; }
        public string IconTheme { get; set; }

        #endregion properties

        #region methods

        private ResourceDictionary dict = new ResourceDictionary();

        public void ChangeTheme()
        {
            if (Theme == "L")
            {
                dict.Source = new Uri("/BadmintonVlaanderen_WPF;component/Dictionaries/Theme1.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(dict);
                Theme = "B";
                IconTheme = "MoonWaningCrescent";
                return;
            }
            if (Theme == "B")
            {
                dict.Source = new Uri("/BadmintonVlaanderen_WPF;component/Dictionaries/Theme2.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(dict);
                Theme = "L";
                IconTheme = "WhiteBalanceSunny";

                return;
            }
        }

        #endregion methods
    }
}
