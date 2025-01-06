using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvenienceStore.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Discount { get; set; }
        public string Note { get; set; }

        public Employee Employee { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }

    public class PurchaseOrder
    {
        public int PurchaseOrderID { get; set; }
        public int SupplierID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }

        public Supplier Supplier { get; set; }
        public Employee Employee { get; set; }
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }

    public class PurchaseOrderDetail
    {
        public int PurchaseOrderDetailID { get; set; }
        public int PurchaseOrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }
        public Product Product { get; set; }
    }

    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string TaxCode { get; set; }
        public string ContactPerson { get; set; }
        public string Status { get; set; }

        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}