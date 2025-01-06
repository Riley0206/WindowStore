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
                    case "RevenueReportPage":
                        MainFrame.Navigate(typeof(RevenueReportPage));
                        break;
                    case "OrderPage":
                        MainFrame.Navigate(typeof(OrderPage));
                        break;
                }

                sender.IsPaneOpen = false;
            }
        }
    }
}