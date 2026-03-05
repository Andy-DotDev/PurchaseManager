using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManager
{
    public enum Category
    {
        Продукты,
        Техника,
        Одежда,
        Прочее
    }
    public class Purchase
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
        public DateTime Date { get; set; }
        public Purchase(string name, decimal price, Category category, DateTime date)
        {
            Name = name;
            Price = price;
            Category = category;
            Date = date;
        }
    }
}
