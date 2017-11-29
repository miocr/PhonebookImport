using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PhonebookImportClient.Views;
using PhonebookImportClient.ViewModels;

namespace PhonebookImportClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow app;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainViewModel context = new MainViewModel();
            app = new MainWindow(context);
            app.Show();
        }
    }
}
