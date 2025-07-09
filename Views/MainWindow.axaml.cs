using ASTEM_DB.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ASTEM_DB.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
