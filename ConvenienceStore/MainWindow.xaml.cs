using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ConvenienceStore.Views;

namespace ConvenienceStore
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string selectedTag = selectedItem.Tag?.ToString();
                if (selectedTag == "InventoryPage")
                {
                    MainFrame.Navigate(typeof(InventoryPage));
                    sender.IsPaneOpen = false; // Tự động đóng NavigationView để tăng không gian hiển thị
                }
            }
        }
    }
}
