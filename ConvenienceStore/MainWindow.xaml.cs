using ConvenienceStore.ViewModels;
using ConvenienceStore.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ConvenienceStore
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void NavigateToInventoryView_Click(object sender, RoutedEventArgs e)
        {
            var inventoryView = new InventoryView
            {
                DataContext = new InventoryViewModel() // Gán DataContext
            };

            contentFrame.Navigate(typeof(InventoryView));
        }
    }
}
