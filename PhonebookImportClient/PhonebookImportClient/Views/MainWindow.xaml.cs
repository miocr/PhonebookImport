using System.Windows;
using PhonebookImportClient.ViewModels;


namespace PhonebookImportClient.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel dataContext)
        {
            this.DataContext = dataContext;
            InitializeComponent();
        }
    }
}
