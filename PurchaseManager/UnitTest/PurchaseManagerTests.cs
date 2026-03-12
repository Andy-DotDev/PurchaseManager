using Microsoft.VisualStudio.TestTools.UnitTesting;
using PurchaseManager;
using System;

namespace UnitTest
{
    [TestClass]
    public class PurchaseManagerTests
    {
        [TestMethod]
        public void PurchaseManager_Constructor_InitializesPurchasesList()
        {
            // Act
            var manager = new PurchaseManager.PurchaseManager();
            manager.ClearAll();

            // Assert
            Assert.IsNotNull(manager.Purchases);
            Assert.AreEqual(0, manager.Purchases.Count);
        }

        [TestMethod]
        public void AddPurchase_ValidPurchase_AddsToList()
        {   
            // Arrange
            var manager = new PurchaseManager.PurchaseManager();
            manager.ClearAll();
            var purchase = new Purchase("Товар1", 100, Category.Продукты, DateTime.Now);
            var initialCount = manager.Purchases.Count;

            // Act
            manager.AddPurchase(purchase);

            // Assert
            Assert.AreEqual(initialCount + 1, manager.Purchases.Count);
            Assert.IsTrue(manager.Purchases.Contains(purchase));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPurchase_NullPurchase_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = new PurchaseManager.PurchaseManager();
            manager.ClearAll();
            // Act
            manager.AddPurchase(null);
        }

        [TestMethod]
        public void RemovePurchase_ValidPurchase_RemovesFromList()
        {
            // Arrange
            var manager = new PurchaseManager.PurchaseManager();
            manager.ClearAll();
            var purchase = new Purchase("Товар1", 100, Category.Продукты, DateTime.Now);
            manager.AddPurchase(purchase);

            // Act
            manager.RemovePurchase(purchase);

            // Assert
            Assert.AreEqual(0, manager.Purchases.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemovePurchase_NullPurchase_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = new PurchaseManager.PurchaseManager();
            manager.ClearAll();

            // Act
            manager.RemovePurchase(null);
        }


        [TestMethod]
        public void AddPurchase_MultiplePurchases_AllAddedCorrectly()
        {
            // Arrange
            var manager = new PurchaseManager.PurchaseManager();
            manager.ClearAll();
            var purchases = new List<Purchase>
            {
                new Purchase("Товар1", 100, Category.Продукты, DateTime.Now),
                new Purchase("Товар2", 200, Category.Техника, DateTime.Now),
                new Purchase("Товар3", 300, Category.Одежда, DateTime.Now)
            };

            // Act
            foreach (var purchase in purchases)
            {
                manager.AddPurchase(purchase);
            }

            // Assert
            Assert.AreEqual(3, manager.Purchases.Count);
            CollectionAssert.AreEqual(purchases, manager.Purchases.ToList());
        }
    }
}
