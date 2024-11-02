using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ConvenienceStore
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(Views.InventoryPage));
        }
    }
}
