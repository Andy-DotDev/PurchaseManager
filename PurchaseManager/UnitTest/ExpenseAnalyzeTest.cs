using PurchaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class ExpenseAnalyzerTests
    {
        [TestMethod]
        public void GetTotalsByCategory_ShouldReturnCorrectSums()
        {
            // Arrange
            var purchases = new List<Purchase>
            {
                new Purchase("Молоко", 80, Category.Продукты, new DateTime(2024, 1, 10)),
                new Purchase("Хлеб", 50, Category.Продукты, new DateTime(2024, 1, 12)),
                new Purchase("Футболка", 1200, Category.Одежда, new DateTime(2024, 1, 15)),
                new Purchase("Наушники", 3500, Category.Техника, new DateTime(2024, 1, 20))
            };
            var analyzer = new ExpenseAnalyzer(purchases);

            // Act
            var result = analyzer.GetTotalsByCategory();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(130m, result["Продукты"]);
            Assert.AreEqual(1200m, result["Одежда"]);
            Assert.AreEqual(3500m, result["Техника"]);
        }

        [TestMethod]
        public void GetTotalByPeriod_ShouldFilterAndSumCorrectly()
        {
            // Arrange
            var purchases = new List<Purchase>
            {
                new Purchase("Январь", 100, Category.Продукты, new DateTime(2024, 1, 15)),
                new Purchase("Февраль", 200, Category.Продукты, new DateTime(2024, 2, 15)),
                new Purchase("Март", 300, Category.Продукты, new DateTime(2024, 3, 15))
            };
            var analyzer = new ExpenseAnalyzer(purchases);
            var start = new DateTime(2024, 1, 1);
            var end = new DateTime(2024, 2, 28);

            // Act
            var result = analyzer.GetTotalByPeriod(start, end);

            // Assert
            Assert.AreEqual(300m, result);
        }

        [TestMethod]
        public void ExportToCsv_ShouldReturnValidFormat()
        {
            // Arrange
            var purchases = new List<Purchase>
            {
                new Purchase("Тест", 99.99m, Category.Прочее, new DateTime(2024, 5, 10))
            };
            var analyzer = new ExpenseAnalyzer(purchases);

            // Act
            var csv = analyzer.ExportToCsv();

            // Assert
            StringAssert.Contains(csv, "Дата;Название;Категория;Цена(руб.)");
            StringAssert.Contains(csv, "2024-05-10;Тест;Прочее;99,99");
        }

        [TestMethod]
        public void GetTotalsByCategory_EmptyList_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var analyzer = new ExpenseAnalyzer(new List<Purchase>());

            // Act
            var result = analyzer.GetTotalsByCategory();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetTotalByPeriod_InvalidRange_ShouldReturnZero()
        {
            // Arrange
            var purchases = new List<Purchase>
            {
                new Purchase("Тест", 100, Category.Прочее, new DateTime(2024, 1, 15))
            };
            var analyzer = new ExpenseAnalyzer(purchases);
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2025, 12, 31);

            // Act
            var result = analyzer.GetTotalByPeriod(start, end);

            // Assert
            Assert.AreEqual(0m, result);
        }
    }
}
