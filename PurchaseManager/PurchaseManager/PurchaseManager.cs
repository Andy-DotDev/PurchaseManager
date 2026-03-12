using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManager
{
    public class PurchaseManager
    {
        public List<Purchase> Purchases { get; private set; }
        public PurchaseManager()
        {
            Purchases = new List<Purchase>();
            LoadPurchases();
        }
        public void AddPurchase(Purchase purchase)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }
            Purchases.Add(purchase);
            SavePurchases();
        }
        public void RemovePurchase(Purchase purchase)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }
            Purchases.Remove(purchase);
            SavePurchases();
        }
        public List<Purchase> GetPurchasesByCategory(Category category)
        {
            return Purchases.Where(p => p.Category == category).ToList();
        }
        private void SavePurchases()
        {
            File.WriteAllLines("purchases.txt", Purchases.Select(p =>
            $"{p.Name}|{p.Price}|{(int)p.Category}|{p.Date.ToString("yyyy-MM-dd HH:mm:ss")}"));
        }
        private void LoadPurchases()
        {
            if (File.Exists("purchases.txt"))
            {
                var lines = File.ReadAllLines("purchases.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        decimal price;
                        if (decimal.TryParse(parts[1], out price))
                        {
                            Category category = (Category)Enum.Parse(typeof(Category), parts[2]);
                            DateTime date;
                            if (DateTime.TryParse(parts[3], out date))
                            {
                                Purchases.Add(new Purchase(parts[0], price, category, date));
                            }
                        }
                    }
                }
            }
        }
        public void ClearAll()
        {
            Purchases.Clear();
            if (File.Exists("purchase.txt"))
            {
                File.Delete("purchase.txt");
            }
        }
    }

}
