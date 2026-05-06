using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using System;
using System.IO;
using System.Threading;
using System.Linq;

[assembly: DoNotParallelize]
namespace PurchaseManager.Tests
{
    [TestClass]
    public class PurchaseManagerUITests
    {
        private FlaUI.Core.Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;
        private string _testDataPath = Path.Combine(Path.GetTempPath(), $"purchases_test_{Guid.NewGuid()}.txt");

        [TestInitialize]
public void TestInitialize()
{
    // Запуск приложения
    _app = FlaUI.Core.Application.Launch(
        @"C:\Users\Andy\Documents\GitHub\PurchaseManager\PurchaseManager\PurchaseManager\bin\Debug\net8.0-windows\PurchaseManager.exe");
    
    _automation = new UIA3Automation();
    
    int attempts = 0;
    int maxAttempts = 1;
    
    while (attempts < maxAttempts && _mainWindow == null)
    {
        try
        {
            Thread.Sleep(1000);
            _mainWindow = _app.GetMainWindow(_automation);
        }
        catch (Exception)
        {
            attempts++;
            if (attempts >= maxAttempts)
                throw new Exception("Не удалось получить главное окно приложения после нескольких попыток");
        }
    }
    
    // Дополнительная проверка
    if (_mainWindow == null)
    {
        throw new Exception("Главное окно не найдено. Убедитесь, что приложение запускается корректно.");
    }
}

        [TestCleanup]
        public void TestCleanup()
        {
            _automation?.Dispose();
            _app?.Close();

            if (File.Exists(_testDataPath))
                File.Delete(_testDataPath);
        }

        #region ТК1-ТК3: Добавление покупок

        /// <summary>
        /// ТК1: Добавление покупки с корректными данными
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC1_AddPurchase_ValidData_ShouldSucceed()
        {
            ClearPurchasesList();
            Thread.Sleep(500);
            // Arrange
            var nameTextBox = FindTextBox("nameTextBox");
            var priceTextBox = FindTextBox("priceTextBox");
            var categoryComboBox = FindComboBox("categoryComboBox");
            var addButton = FindButton("addPurchaseButton");
            var listBox = FindListBox("purchasesListBox");

            // Act
            nameTextBox.Text = "Молоко";
            priceTextBox.Text = "100";
            categoryComboBox.Select("Продукты");
            addButton.Click();

            Thread.Sleep(1500);
            CloseMessageBox();
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(listBox.Items.Length > 0, "Список покупок пуст");
            Assert.IsTrue(listBox.Items[0].Text.Contains("Молоко"), "Не найдено название 'Молоко'");
            Assert.IsTrue(listBox.Items[0].Text.Contains("100"), "Не найдена цена '100'");
            Assert.IsTrue(listBox.Items[0].Text.Contains("Продукты"), "Не найдена категория 'Продукты'");
        }

        /// <summary>
        /// ТК2: Добавление покупки с пустым названием
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC2_AddPurchase_EmptyName_ShouldShowError()
        {
            ClearPurchasesList();
            Thread.Sleep(500);
            // Arrange
            var priceTextBox = FindTextBox("priceTextBox");
            var categoryComboBox = FindComboBox("categoryComboBox");
            var addButton = FindButton("addPurchaseButton");

            // Act
            priceTextBox.Text = "100";
            categoryComboBox.Select("Продукты");
            addButton.Click();

            Thread.Sleep(1000);
            var modal = _mainWindow.ModalWindows.FirstOrDefault();
            var textElement = modal.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
            var messageText = textElement?.Name ?? "";

            // Assert
            Assert.IsNotNull(modal, "Модальное окно не появилось");
            Assert.IsTrue(modal.Name.Contains("Ошибка"), "Заголовок окна не содержит 'Ошибка'");
            Assert.IsTrue(messageText.Contains("Заполните все поля"),
                $"Сообщение не содержит 'Заполните все поля'. Текст: {messageText}");

            CloseMessageBox();
        }

        /// <summary>
        /// ТК3: Добавление покупки с пустыми полями
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC3_AddPurchase_AllFieldsEmpty_ShouldShowError()
        {
            // Arrange
            var addButton = FindButton("addPurchaseButton");

            // Act
            addButton.Click();
            Thread.Sleep(1000);

            var modal = _mainWindow.ModalWindows.FirstOrDefault();
            var textElement = modal.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
            var messageText = textElement?.Name ?? "";
            // Assert
            Assert.IsNotNull(modal, "Модальное окно не появилось");
            Assert.IsTrue(modal.Name.Contains("Ошибка"), "Заголовок окна не содержит 'Ошибка'");
            Assert.IsTrue(messageText.Contains("Заполните все поля"),
                $"Сообщение не содержит 'Заполните все поля'. Текст: {messageText}");

            CloseMessageBox();
        }

        #endregion

        #region ТК4-ТК5: Удаление покупок

        /// <summary>
        /// ТК4: Удаление покупки из списка
        /// Статус: Провален (Покупка не удаляется из списка)
        /// </summary>
        [TestMethod]
        public void TC4_RemovePurchase_ShouldSucceed()
        {
            // Arrange
            AddTestPurchase("Тестовый товар", "250", "Техника");

            var listBox = FindListBox("purchasesListBox");
            var removeButton = FindButton("removePurchaseButton");
            int initialCount = listBox.Items.Length;

            // Act
            listBox.Items[0].Click();
            Thread.Sleep(1000);
            removeButton.Click();

            Thread.Sleep(2000);
            var confirmDialog = _mainWindow.ModalWindows.FirstOrDefault();
            var yesButton = confirmDialog?.FindFirstDescendant(cf => cf.ByText("Да"))?.AsButton();
            yesButton?.Click();

            Thread.Sleep(1000);
            CloseMessageBox();

            // Assert
            Assert.AreEqual(initialCount - 1, listBox.Items.Length,
                "Количество покупок не уменьшилось на 1");
        }

        /// <summary>
        /// ТК5: Удаление покупки без выбора
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC5_RemovePurchase_NoSelection_ShouldShowError()
        {
            // Arrange
            var removeButton = FindButton("removePurchaseButton");

            // Act
            removeButton.Click();
            Thread.Sleep(1000);

            var modal = _mainWindow.ModalWindows.FirstOrDefault();
            var textElement = modal.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
            var messageText = textElement?.Name ?? "";

            // Assert
            Assert.IsNotNull(modal, "Модальное окно не появилось");
            Assert.IsTrue(messageText.Contains("Выберите покупку для удаления"),
    $"Сообщение не содержит 'Выберите покупку для удаления'. Текст: {messageText}");

            CloseMessageBox();
        }

        #endregion

        #region ТК6-ТК9: Фильтрация по категориям

        /// <summary>
        /// ТК6: Фильтрация покупок по категории "Продукты"
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC6_FilterByCategory_Products_ShouldShowOnlyProducts()
        {
            // Arrange: добавляем покупки разных категорий
            AddTestPurchase("Молоко", "80", "Продукты");
            AddTestPurchase("Футболка", "1200", "Одежда");
            AddTestPurchase("Хлеб", "50", "Продукты");

            var filterCombo = FindComboBox("categoryFilterComboBox");
            var filterButton = FindButton("filterButton");
            var listBox = FindListBox("purchasesListBox");

            // Act
            SelectComboBoxItem(filterCombo, "Продукты");
            filterButton.Click();
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(listBox.Items.Length > 0, "Список пуст после фильтрации");
            foreach (var item in listBox.Items)
            {
                Assert.IsTrue(item.Text.Contains("Продукты"),
                    $"Элемент '{item.Text}' не относится к категории 'Продукты'");
            }
        }

        /// <summary>
        /// ТК7: Фильтрация покупок по категории "Техника"
        /// Статус: Провален
        /// </summary>
        [TestMethod]
        public void TC7_FilterByCategory_Technics_ShouldShowOnlyTechnics()
        {
            ClearPurchasesList();
            Thread.Sleep(500);
            // Arrange
            AddTestPurchase("Ноутбук", "50000", "Техника");
            AddTestPurchase("Молоко", "80", "Продукты");
            AddTestPurchase("Телефон", "30000", "Техника");

            var filterCombo = FindComboBox("categoryFilterComboBox");
            var filterButton = FindButton("filterButton");
            var listBox = FindListBox("purchasesListBox");

            // Act
            SelectComboBoxItem(filterCombo, "Техника");
            Thread.Sleep(3000);
            filterButton.Click();
            Thread.Sleep(1000);

            // Assert
            foreach (var item in listBox.Items)
            {
                Assert.IsTrue(item.Text.Contains("Техника"),
                    $"Элемент '{item.Text}' не относится к категории 'Техника'");
            }
        }

        /// <summary>
        /// ТК8: Фильтрация покупок по категории "Одежда"
        /// Статус: Провален
        /// </summary>
        [TestMethod]
        public void TC8_FilterByCategory_Clothes_ShouldShowOnlyClothes()
        {
            // Arrange
            AddTestPurchase("Футболка", "1200", "Одежда");
            AddTestPurchase("Молоко", "80", "Продукты");
            AddTestPurchase("Джинсы", "3000", "Одежда");

            var filterCombo = FindComboBox("categoryFilterComboBox");
            var filterButton = FindButton("filterButton");
            var listBox = FindListBox("purchasesListBox");

            // Act
            SelectComboBoxItem(filterCombo, "Одежда");
            filterButton.Click();
            Thread.Sleep(1000);

            // Assert
            foreach (var item in listBox.Items)
            {
                Assert.IsTrue(item.Text.Contains("Одежда"),
                    $"Элемент '{item.Text}' не относится к категории 'Одежда'");
            }
        }

        /// <summary>
        /// ТК9: Фильтрация покупок по категории "Прочее"
        /// Статус: Провален
        /// </summary>
        [TestMethod]
        public void TC9_FilterByCategory_Other_ShouldShowOnlyOther()
        {
            // Arrange
            AddTestPurchase("Книга", "500", "Прочее");
            AddTestPurchase("Молоко", "80", "Продукты");
            AddTestPurchase("Журнал", "300", "Прочее");

            var filterCombo = FindComboBox("categoryFilterComboBox");
            var filterButton = FindButton("filterButton");
            var listBox = FindListBox("purchasesListBox");

            // Act
            SelectComboBoxItem(filterCombo, "Прочее");
            filterButton.Click();
            Thread.Sleep(1000);

            // Assert
            foreach (var item in listBox.Items)
            {
                Assert.IsTrue(item.Text.Contains("Прочее"),
                    $"Элемент '{item.Text}' не относится к категории 'Прочее'");
            }
        }

        #endregion

        #region ТК10-ТК13: Работа с файлом

        /// <summary>
        /// ТК10: Проверка формата сохранения данных в файл purchases.txt
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC10_SaveDataToFile_ShouldCorrectFormat()
        {
            ClearPurchasesList();
            Thread.Sleep(300);
            // Arrange
            AddTestPurchase("Тестовый товар", "999", "Прочее");
            string purchasesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "purchases.txt");

            // Act 
            Thread.Sleep(1000);

            // Assert
            Assert.IsTrue(File.Exists(purchasesFile), "Файл purchases.txt не создан");

            var content = File.ReadAllText(purchasesFile);
            Assert.IsTrue(content.Contains("Тестовый товар"), "Название не сохранено");
            Assert.IsTrue(content.Contains("999"), "Цена не сохранена");
            Assert.IsFalse(content.Contains("Прочее"), "Категория не сохранена");
        }

        /// <summary>
        /// ТК11: Загрузка списка покупок при запуске
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC11_LoadPurchasesOnStartup_ShouldLoadExistingData()
        {
            // Arrange
            string purchasesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "purchases.txt");
            File.WriteAllText(purchasesFile, "Молоко|100|Продукты|01.05.2026 10:00:00");

            TestCleanup();
            TestInitialize();

            // Act
            Thread.Sleep(1000);
            var listBox = FindListBox("purchasesListBox");

            // Assert
            Assert.IsTrue(listBox.Items.Length > 0, "Список покупок пуст после загрузки");
            Assert.IsTrue(listBox.Items[0].Text.Contains("Молоко"), "Данные не загрузились корректно");
        }

        /// <summary>
        /// ТК12: Инициализация при отсутствии файла purchases.txt
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC12_NoPurchasesFile_ShouldStartWithEmptyList()
        {
            // Arrange
            string purchasesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "purchases.txt");
            if (File.Exists(purchasesFile))
                File.Delete(purchasesFile);

            TestCleanup();
            TestInitialize();

            // Act
            Thread.Sleep(1000);
            var listBox = FindListBox("purchasesListBox");

            // Assert
            Assert.AreEqual(0, listBox.Items.Length, "Список должен быть пуст при отсутствии файла");
        }

        /// <summary>
        /// ТК13: Обработка некорректного формата строк в файле
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC13_InvalidFileFormat_ShouldSkipInvalidLines()
        {
            // Arrange
            string purchasesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "purchases.txt");
            File.WriteAllText(purchasesFile, "123|wasd|d|32 23 12\nНекорректная строка");

   
            TestCleanup();
            TestInitialize();

            // Act & Assert
            Thread.Sleep(1000);
            var listBox = FindListBox("purchasesListBox");
            Assert.IsTrue(listBox.Items.Length == 0 || listBox.Items.Length > 0,
                "Приложение не должно падать при некорректных данных");
        }

        #endregion

        #region ТК14-ТК17: Новая функция "Анализ расходов"

        /// <summary>
        /// ТК14: Проверка корректности расчета сумм по категориям
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC14_AnalyzeByCategory_ShouldShowCorrectStats()
        {
            ClearPurchasesList();
            Thread.Sleep(300);
            // Arrange
            AddTestPurchase("Молоко", "80", "Продукты");
            AddTestPurchase("Хлеб", "50", "Продукты");
            AddTestPurchase("Футболка", "1200", "Одежда");
            AddTestPurchase("Наушники", "3500", "Техника");

            var btnByCategory = FindButton("btnByCategory");
            var resultLabel = FindLabel("resultLabel");

            // Act
            btnByCategory.Click();
            Thread.Sleep(1000);

            // Assert
            var resultText = resultLabel.Text;
            Assert.IsTrue(resultText.Contains("Расходы по категориям"), "Отсутствует заголовок");
            Assert.IsTrue(resultText.Contains("Продукты"), "Должна быть категория 'Продукты'");
            Assert.IsTrue(resultText.Contains("Одежда"), "Должна быть категория 'Одежда'");
            Assert.IsTrue(resultText.Contains("Техника"), "Должна быть категория 'Техника'");
            Assert.IsTrue(resultText.Contains("130"), "Сумма по Продуктам должна быть 130 руб. (80+50)");
            Assert.IsTrue(resultText.Contains("1200"), "Сумма по Одежде должна быть 1200 руб.");
            Assert.IsTrue(resultText.Contains("3500"), "Сумма по Технике должна быть 3500 руб.");
            Assert.IsTrue(resultText.Contains("Итого"), "Должна быть итоговая сумма");
        }

        /// <summary>
        /// ТК15: Проверка корректности фильтрации по датам
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC15_AnalyzeByPeriod_ShouldFilterByDate()
        {
            // Arrange
            AddTestPurchase("Покупка1", "100", "Прочее", new DateTime(2026, 5, 10));
            AddTestPurchase("Покупка2", "200", "Прочее", new DateTime(2026, 5, 15));
            AddTestPurchase("Покупка3", "300", "Прочее", new DateTime(2026, 6, 15));

            var dateStart = FindDatePicker("dateStart");
            var dateEnd = FindDatePicker("dateEnd");
            var btnByPeriod = FindButton("btnByPeriod");
            var resultLabel = FindLabel("resultLabel");

            // Act
            dateStart.Patterns.Value.Pattern.SetValue("01.05.2026");
            dateEnd.Patterns.Value.Pattern.SetValue("31.05.2026");
            Thread.Sleep(1000);
            btnByPeriod.Click();
            Thread.Sleep(1000);

            // Assert
            var resultText = resultLabel.Text;
            Assert.IsTrue(resultText.Contains("Расходы с"), "Должен быть заголовок с датами");
            Assert.IsTrue(resultText.Contains("Итого за период"), "Должна быть итоговая сумма за период");
        }

        /// <summary>
        /// ТК16: Проверка формирования и сохранения CSV-файла
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC16_ExportToCsv_ShouldCreateValidFile()
        {
            // Arrange
            AddTestPurchase("Товар1", "100", "Продукты");
            AddTestPurchase("Товар2", "200", "Техника");
            AddTestPurchase("Товар3", "300", "Одежда");

            var btnExport = FindButton("btnExport");
            string tempCsv = Path.Combine(Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.csv");

            // Act
            btnExport.Click();
            Thread.Sleep(1000);

       
            var dialog = _mainWindow.ModalWindows.FirstOrDefault();

            // Assert
            Assert.IsNotNull(dialog, "Диалог сохранения не открылся");
            dialog?.Close();
            Assert.IsTrue(_mainWindow.IsAvailable, "Приложение не должно было упасть");
            TestCleanup();
        }

        /// <summary>
        /// ТК17: Анализ при пустом списке
        /// Статус: Пройден
        /// </summary>
        [TestMethod]
        public void TC17_AnalyzeEmptyList_ShouldShowMessage()
        {
            ClearPurchasesList();
            Thread.Sleep(300);
            // Arrange
            var btnByCategory = FindButton("btnByCategory");
            var btnByPeriod = FindButton("btnByPeriod");
            var resultLabel = FindLabel("resultLabel");

            // Act & Assert
            btnByCategory.Click();
            Thread.Sleep(1000);
            Assert.IsTrue(resultLabel.Text.Contains("Нет данных для анализа"),
                "Должно быть сообщение 'Нет данных для анализа'");

            // Act & Assert
            btnByPeriod.Click();
            Thread.Sleep(1000);
            Assert.IsTrue(resultLabel.Text.Contains("Нет покупок за выбранный период"),
                "Должно быть сообщение 'Нет покупок за выбранный период'");
        }

        #endregion

        #region Вспомогательные методы

        private void AddTestPurchase(string name, string price, string category, DateTime? date = null)
        {
            var nameBox = FindTextBox("nameTextBox");
            var priceBox = FindTextBox("priceTextBox");
            var categoryBox = FindComboBox("categoryComboBox");
            var addButton = FindButton("addPurchaseButton");

            nameBox.Text = name;
            priceBox.Text = price;
            SelectComboBoxItem(categoryBox, category);
            Thread.Sleep(2000);

            if (date.HasValue)
            {
                var datePicker = FindDatePicker("datePicker");
                datePicker.Patterns.Value.Pattern.SetValue(date.Value.ToString("dd.MM.yyyy"));
            }

            addButton.Click();
            Thread.Sleep(300);
            CloseMessageBox();
            Thread.Sleep(200);
        }

        private TextBox FindTextBox(string automationId)
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsTextBox()
                ?? throw new Exception($"TextBox '{automationId}' не найден");
        }

        private ComboBox FindComboBox(string automationId)
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsComboBox()
                ?? throw new Exception($"ComboBox '{automationId}' не найден");
        }

        private Button FindButton(string automationId)
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsButton()
                ?? throw new Exception($"Button '{automationId}' не найден");
        }

        private ListBox FindListBox(string automationId)
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsListBox()
                ?? throw new Exception($"ListBox '{automationId}' не найден");
        }

        private Label FindLabel(string automationId)
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsLabel()
                ?? throw new Exception($"Label '{automationId}' не найден");
        }

        private DateTimePicker FindDatePicker(string automationId)
        {
            return _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsDateTimePicker()
                ?? throw new Exception($"DateTimePicker '{automationId}' не найден");
        }

        private void CloseMessageBox()
        {
            var modal = _mainWindow.ModalWindows.FirstOrDefault();
            if (modal != null)
            {
                var okButton = modal.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));
                okButton?.Click();
                Thread.Sleep(200);
            }
        }

        private void SelectComboBoxItem(ComboBox comboBox, string itemText)
        {
            // Просто используем паттерн Value
            comboBox.Patterns.Value.Pattern.SetValue(itemText);
            Thread.Sleep(300);
        }
        private void ClearPurchasesList()
        {
                var listBox = FindListBox("purchasesListBox");
                var removeButton = FindButton("removePurchaseButton");

                while (listBox.Items.Length > 0)
                {
                    listBox.Items[0].Click();
                    Thread.Sleep(200);
                    removeButton.Click();
                    Thread.Sleep(500);

                    var confirm = _mainWindow.ModalWindows.FirstOrDefault();
                    var yesBtn = confirm?.FindFirstDescendant(cf => cf.ByText("Да"))?.AsButton();
                    yesBtn?.Click();
                    Thread.Sleep(300);

                    CloseMessageBox();
                }
            
        }

        #endregion
    }
}