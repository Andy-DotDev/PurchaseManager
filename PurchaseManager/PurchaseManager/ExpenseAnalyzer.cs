using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseManager
{
    public class ExpenseAnalyzer
    {
        private readonly List<Purchase> _purchases;

        public ExpenseAnalyzer(List<Purchase> purchases)
        {
            _purchases = purchases ?? new List<Purchase>();
        }

        /// 1. Статистика по категориям (сумма расходов в каждой категории)
        public Dictionary<string, decimal> GetTotalsByCategory()
        {
            return _purchases
                .GroupBy(p => p.Category.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Price));
        }

        /// 2. Статистика за период в диапазоне дат
        public decimal GetTotalByPeriod(DateTime startDate, DateTime endDate)
        {
            return _purchases
                .Where(p => p.Date >= startDate.Date && p.Date <= endDate.Date)
                .Sum(p => p.Price);
        }

        /// 3. Детализация по категориям за период
        public Dictionary<string, decimal> GetTotalsByCategoryForPeriod(DateTime startDate, DateTime endDate)
        {
            return _purchases
                .Where(p => p.Date >= startDate.Date && p.Date <= endDate.Date)
                .GroupBy(p => p.Category.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Price));
        }

        /// 4. Экспорт в CSV
        public string ExportToCsv()
        {
            var sb = new StringBuilder("Дата;Название;Категория;Цена(руб.)\n");
            foreach (var p in _purchases.OrderBy(x => x.Date))
            {
                sb.AppendLine($"{p.Date:yyyy-MM-dd};{p.Name};{p.Category};{p.Price}");
            }
            return sb.ToString();
        }

        /// 5. Экспорт статистики по категориям в CSV
        public string ExportCategoryStatsToCsv()
        {
            var stats = GetTotalsByCategory();
            var sb = new StringBuilder("Категория;Сумма(руб.)\n");
            foreach (var item in stats)
            {
                sb.AppendLine($"{item.Key};{item.Value}");
            }
            return sb.ToString();
        }
    }
}
