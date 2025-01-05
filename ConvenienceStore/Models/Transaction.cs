using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvenienceStore.Models
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public int? ReferenceID { get; set; }
        public string ReferenceType { get; set; }
        public string PaymentMethod { get; set; }
    }
}
