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

                switch (selectedTag)
                {
                    case "InventoryPage":
                        MainFrame.Navigate(typeof(InventoryPage));
                        break;
                    case "EmployeePage":
                        MainFrame.Navigate(typeof(EmployeePage));
                        break;
                }

                sender.IsPaneOpen = false;
            }
        }
    }
}