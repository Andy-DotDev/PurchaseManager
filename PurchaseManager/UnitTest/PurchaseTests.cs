using Microsoft.VisualStudio.TestTools.UnitTesting;
using PurchaseManager;
using System;

namespace UnitTest
{
    [TestClass]
    public class PurchaseTests
    {
        [TestMethod]
        public void Purchase_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "Телефон";
            var price = 29999.99m;
            var category = Category.Техника;
            var date = new DateTime(2024, 3, 15);

            // Act
            var purchase = new Purchase(name, price, category, date);

            // Assert
            Assert.AreEqual(name, purchase.Name);
            Assert.AreEqual(price, purchase.Price);
            Assert.AreEqual(category, purchase.Category);
            Assert.AreEqual(date, purchase.Date);
        }

        [TestMethod]
        public void Purchase_Name_CanBeSetAndGet()
        {
            // Arrange
            var purchase = new Purchase("Тест", 100, Category.Продукты, DateTime.Now);
            var expectedName = "Ноутбук";

            // Act
            purchase.Name = expectedName;
            var actualName = purchase.Name;

            // Assert
            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void Purchase_Price_CanBeSetAndGet()
        {
            // Arrange
            var purchase = new Purchase("Тест", 100, Category.Продукты, DateTime.Now);
            var expectedPrice = 15000.50m;

            // Act
            purchase.Price = expectedPrice;
            var actualPrice = purchase.Price;

            // Assert
            Assert.AreEqual(expectedPrice, actualPrice);
        }

        [TestMethod]
        public void Purchase_Category_CanBeSetAndGet()
        {
            // Arrange
            var purchase = new Purchase("Тест", 100, Category.Продукты, DateTime.Now);
            var expectedCategory = Category.Одежда;

            // Act
            purchase.Category = expectedCategory;
            var actualCategory = purchase.Category;

            // Assert
            Assert.AreEqual(expectedCategory, actualCategory);
        }

        [TestMethod]
        public void Purchase_Date_CanBeSetAndGet()
        {
            // Arrange
            var purchase = new Purchase("Тест", 100, Category.Продукты, DateTime.Now);
            var expectedDate = DateTime.Now;

            // Act
            purchase.Date = expectedDate;
            var actualDate = purchase.Date;

            // Assert
            Assert.AreEqual(expectedDate, actualDate);
        }
    }
}
