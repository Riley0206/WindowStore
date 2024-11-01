using Microsoft.UI.Xaml.Controls;
using ConvenienceStore.ViewModels;

namespace ConvenienceStore.Views
{
    public sealed partial class InventoryView : Page
    {
        public InventoryView()
        {
            this.InitializeComponent();
            this.DataContext = new InventoryViewModel(); 
        }
    }
}
