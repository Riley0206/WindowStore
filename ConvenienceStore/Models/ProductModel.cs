using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvenienceStore.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string Brand { get; set; }
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice => Price.ToString("C");

    }
}
