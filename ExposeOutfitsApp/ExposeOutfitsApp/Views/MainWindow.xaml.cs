using ExposeOutfitsApp.Helpers;
using ExposeOutfitsApp.ViewModels;
using System.Windows;

namespace ExposeOutfitsApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            var url = PasteUrlTextBox.Text;

            // Parse URL
            var (roomIds, userProducts) = ImvuUrlParser.Parse(url);

            // Load everything asynchronously
            await _vm.LoadAsync(roomIds, userProducts);
        }
    }
}
