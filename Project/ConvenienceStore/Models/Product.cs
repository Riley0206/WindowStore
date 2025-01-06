using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvenienceStore.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string Brand { get; set; }
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public string Unit { get; set; }    
        public Category Category { get; set; }
    }
}
