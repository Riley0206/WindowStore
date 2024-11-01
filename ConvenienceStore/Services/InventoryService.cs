using System.Collections.Generic;
using System.Threading.Tasks;
using ConvenienceStore.Models;
using ConvenienceStore.Helpers;

namespace ConvenienceStore.Services
{
    public class InventoryService
    {
        private readonly DatabaseHelper _databaseHelper;

        public InventoryService(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public async Task<IEnumerable<ProductModel>> GetInventory()
        {
            try
            {
                return await _databaseHelper.GetProductsAsync();
            }
            catch
            {
                // Xử lý lỗi và trả về một danh sách trống nếu có lỗi
                return new List<ProductModel>();
            }
        }

        public void PlaceOrder(int productId)
        {
            // Phương thức xử lý đặt hàng
        }
    }
}
