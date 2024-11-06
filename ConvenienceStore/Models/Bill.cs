using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvenienceStore.Models
{
    public class Bill
    {
        public int PurchaseOrderID { get; set; }
        public int SupplierID { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
    }
}
