using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using WPF.ViewModels;
using WPF.Views;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HomeViewModel _homeViewModel;

        public HomeViewModel HomeViewModel
        {
            get { return _homeViewModel; }
            set { _homeViewModel = value; }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainViewModel mvModel = new MainViewModel();
            MainView main = new MainView
            {
                DataContext = mvModel
            };
            main.Show();

            HomeViewModel = new HomeViewModel();
            main.mainPanel.Children.Clear();
            Home homeViewStartup = new Home
            {
                DataContext = HomeViewModel
            };
            main.mainPanel.Children.Add(homeViewStartup);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));
            base.OnStartup(e);
        }
    }
}
