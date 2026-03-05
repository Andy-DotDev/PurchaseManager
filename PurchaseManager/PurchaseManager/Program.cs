namespace PurchaseManager
{
    public class PurchaseForm : Form
    {
        private PurchaseManager purchaseManager;
        private TextBox nameTextBox;
        private TextBox priceTextBox;
        private ComboBox categoryComboBox;
        private DateTimePicker datePicker;
        private Button addPurchaseButton;
        private Button removePurchaseButton;
        private ComboBox categoryFilterComboBox;
        private Button filterButton;
        private ListBox purchasesListBox;
        public PurchaseForm()
        {
            this.Text = "Управление покупками";
            this.Width = 600;
            this.Height = 500;
            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Width = 150,
                PlaceholderText = "Название"
            };
            priceTextBox = new TextBox
            {
                Location = new System.Drawing.Point(170, 10),
                Width = 100,
                PlaceholderText = "Цена"
            };
            categoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(280, 10),
                Width = 100,
                Items = { "Продукты", "Техника", "Одежда", "Прочее" }
            };
            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(390, 10)
            };
            addPurchaseButton = new Button
            {
                Location = new System.Drawing.Point(10, 40),
                Text = "Добавить",
                Width = 100
            };
            addPurchaseButton.Click += AddPurchaseButton_Click;
            removePurchaseButton = new Button
            {
                Location = new System.Drawing.Point(120, 40),
                Text = "Удалить",
                Width = 100
            };
            removePurchaseButton.Click += RemovePurchaseButton_Click;
            categoryFilterComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(10, 70),
                Width = 100,
                Items = { "Все", "Продукты", "Техника", "Одежда", "Прочее" }
            };
            filterButton = new Button
            {
                Location = new System.Drawing.Point(120, 70),
                Text = "Фильтровать",
                Width = 100
            };
            filterButton.Click += FilterButton_Click;
            purchasesListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 100),
                Width = 560,
                Height = 300
            };
            this.Controls.Add(nameTextBox);
            this.Controls.Add(priceTextBox);
            this.Controls.Add(categoryComboBox);
            this.Controls.Add(datePicker);
            this.Controls.Add(addPurchaseButton);
            this.Controls.Add(removePurchaseButton);
            this.Controls.Add(categoryFilterComboBox);
            this.Controls.Add(filterButton);
            this.Controls.Add(purchasesListBox);
            purchaseManager = new PurchaseManager();
            UpdatePurchasesList();
        }
        private void UpdatePurchasesList()
        {
            purchasesListBox.Items.Clear();
            foreach (var purchase in purchaseManager.Purchases)
            {
                purchasesListBox.Items.Add($"{purchase.Name} - {purchase.Price} руб.({ purchase.Category})");
            }
        }
        private void AddPurchaseButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text) || string.IsNullOrEmpty(priceTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            decimal price;
            if (!decimal.TryParse(priceTextBox.Text, out price))
            {
                MessageBox.Show("Неверный формат цены!");
                return;
            }
            Category category = (Category)Enum.Parse(typeof(Category),
            categoryComboBox.SelectedItem.ToString());
            DateTime date = datePicker.Value;
            Purchase newPurchase = new Purchase(nameTextBox.Text, price, category, date);
            try
            {
                purchaseManager.AddPurchase(newPurchase);
                nameTextBox.Clear();
                priceTextBox.Clear();
                UpdatePurchasesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemovePurchaseButton_Click(object sender, EventArgs e)
        {
            if (purchasesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите покупку для удаления!");
                return;
            }
            string selectedItem = purchasesListBox.SelectedItem.ToString();
            string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string name = parts[0].Trim();
                decimal price;
                if (decimal.TryParse(parts[1].Split(' ')[0], out price))
                {
                    var purchaseToRemove = purchaseManager.Purchases.Find(p => p.Name ==
                    name && p.Price == price);
                    if (purchaseToRemove != null)
                    {
                        try
                        {
                            purchaseManager.RemovePurchase(purchaseToRemove);
                            UpdatePurchasesList();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }
        private void FilterButton_Click(object sender, EventArgs e)
        {
            Category category = categoryFilterComboBox.SelectedIndex == 0 ?
            Category.Продукты : (Category)Enum.Parse(typeof(Category),
            categoryFilterComboBox.SelectedItem.ToString());
            var filteredPurchases = purchaseManager.GetPurchasesByCategory(category);
            purchasesListBox.Items.Clear();
            foreach (var purchase in filteredPurchases)
            {
                purchasesListBox.Items.Add($"{purchase.Name} - {purchase.Price} руб.({ purchase.Category})");
            }
        }
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PurchaseForm());
        }
    }

}