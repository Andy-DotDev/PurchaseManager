namespace PurchaseManager
{
    public class PurchaseForm : Form
    {
        private PurchaseManager purchaseManager;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label priceLabel;
        private TextBox priceTextBox;
        private Label categoryLabel;
        private ComboBox categoryComboBox;
        private Label dateLabel;
        private DateTimePicker datePicker;
        private Button addPurchaseButton;
        private Button removePurchaseButton;
        private Label filterLabel;
        private ComboBox categoryFilterComboBox;
        private Button filterButton;
        private ListBox purchasesListBox;
        private List<Purchase> currentDisplayedPurchases;

        public PurchaseForm()
        {
            this.Text = "Управление покупками";
            this.Width = 650;
            this.Height = 550;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Label
            nameLabel = new Label
            {
                Location = new System.Drawing.Point(12, 15),
                Text = "Название:",
                AutoSize = true
            };

            priceLabel = new Label
            {
                Location = new System.Drawing.Point(180, 15),
                Text = "Цена:",
                AutoSize = true
            };

            categoryLabel = new Label
            {
                Location = new System.Drawing.Point(310, 15),
                Text = "Категория:",
                AutoSize = true
            };

            dateLabel = new Label
            {
                Location = new System.Drawing.Point(440, 15),
                Text = "Дата:",
                AutoSize = true
            };

            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(12, 35),
                Width = 155,
                PlaceholderText = "Название"
            };

            priceTextBox = new TextBox
            {
                Location = new System.Drawing.Point(180, 35),
                Width = 115,
                PlaceholderText = "Цена"
            };

            categoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(310, 35),
                Width = 115,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            categoryComboBox.Items.AddRange(new object[] { "Продукты", "Техника", "Одежда", "Прочее" });
            categoryComboBox.SelectedIndex = 0;

            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(440, 35),
                Width = 170,
                Format = DateTimePickerFormat.Short
            };

            addPurchaseButton = new Button
            {
                Location = new System.Drawing.Point(12, 70),
                Text = "Добавить",
                Width = 100,
                Height = 30
            };
            addPurchaseButton.Click += AddPurchaseButton_Click;

            removePurchaseButton = new Button
            {
                Location = new System.Drawing.Point(125, 70),
                Text = "Удалить",
                Width = 100,
                Height = 30
            };
            removePurchaseButton.Click += RemovePurchaseButton_Click;

            filterLabel = new Label
            {
                Location = new System.Drawing.Point(250, 75),
                Text = "Фильтр по категории:",
                AutoSize = true
            };

            categoryFilterComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(400, 72),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            categoryFilterComboBox.Items.AddRange(new object[] { "Все категории", "Продукты", "Техника", "Одежда", "Прочее" });
            categoryFilterComboBox.SelectedIndex = 0;

            filterButton = new Button
            {
                Location = new System.Drawing.Point(530, 70),
                Text = "Фильтровать",
                Width = 90,
                Height = 30
            };
            filterButton.Click += FilterButton_Click;

            purchasesListBox = new ListBox
            {
                Location = new System.Drawing.Point(12, 110),
                Width = 608,
                Height = 380,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(priceLabel);
            this.Controls.Add(priceTextBox);
            this.Controls.Add(categoryLabel);
            this.Controls.Add(categoryComboBox);
            this.Controls.Add(dateLabel);
            this.Controls.Add(datePicker);
            this.Controls.Add(addPurchaseButton);
            this.Controls.Add(removePurchaseButton);
            this.Controls.Add(filterLabel);
            this.Controls.Add(categoryFilterComboBox);
            this.Controls.Add(filterButton);
            this.Controls.Add(purchasesListBox);

            purchaseManager = new PurchaseManager();
            currentDisplayedPurchases = new List<Purchase>();
            UpdatePurchasesList();
        }

        private void UpdatePurchasesList()
        {
            purchasesListBox.Items.Clear();
            currentDisplayedPurchases.Clear();

            foreach (var purchase in purchaseManager.Purchases)
            {
                purchasesListBox.Items.Add($"{purchase.Name} - {purchase.Price} руб. ({purchase.Category})");
                currentDisplayedPurchases.Add(purchase);
            }
        }

        private void AddPurchaseButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text) || string.IsNullOrEmpty(priceTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal price;
            if (!decimal.TryParse(priceTextBox.Text, out price))
            {
                MessageBox.Show("Неверный формат цены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (price <= 0)
            {
                MessageBox.Show("Цена должна быть положительной!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Category category = (Category)Enum.Parse(typeof(Category), categoryComboBox.SelectedItem.ToString());
            DateTime date = datePicker.Value;
            Purchase newPurchase = new Purchase(nameTextBox.Text, price, category, date);
            try
            {
                purchaseManager.AddPurchase(newPurchase);
                nameTextBox.Clear();
                priceTextBox.Clear();
                UpdatePurchasesList();
                MessageBox.Show("Покупка успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemovePurchaseButton_Click(object sender, EventArgs e)
        {
            if (purchasesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите покупку для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int selectedItem = purchasesListBox.SelectedIndex;
            DialogResult result = MessageBox.Show(
               $"Вы уверены, что хотите удалить покупку?\n{purchasesListBox.SelectedItem}",
               "Подтверждение удаления",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {

                    Purchase purchaseToRemove = currentDisplayedPurchases[selectedItem];

                    purchaseManager.RemovePurchase(purchaseToRemove);

                    UpdatePurchasesList();

                    MessageBox.Show("Покупка успешно удалена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (categoryFilterComboBox.SelectedIndex == 0)
            {
                UpdatePurchasesList();
                return;
            }

            Category category = (Category)Enum.Parse(typeof(Category), categoryFilterComboBox.SelectedItem.ToString());
            var filteredPurchases = purchaseManager.GetPurchasesByCategory(category);
            purchasesListBox.Items.Clear();
            foreach (var purchase in filteredPurchases)
            {
                purchasesListBox.Items.Add($"{purchase.Name} - {purchase.Price} руб. ({purchase.Category})");
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