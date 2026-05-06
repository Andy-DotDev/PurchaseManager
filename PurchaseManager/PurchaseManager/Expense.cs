using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManager
{
    public class Expense
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }
}
