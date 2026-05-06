using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace PurchaseManager
{
    public class PurchaseForm : Form
    {
        #region Ads
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
        private DateTimePicker dateStart;
        private DateTimePicker dateEnd;
        private Label resultLabel;
        private Chart expenseChart;
        #endregion

        public PurchaseForm()
        {
            #region MainForm
            this.Text = "Управление покупками";
            this.Width = 650;
            this.Height = 970;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Label
            nameLabel = new Label
            {
                Location = new System.Drawing.Point(12, 15),
                Text = "Название:",
                AutoSize = true,
                Name = "nameLabel"
            };

            priceLabel = new Label
            {
                Location = new System.Drawing.Point(180, 15),
                Text = "Цена:",
                AutoSize = true,
                Name = "priceLabel"
            };

            categoryLabel = new Label
            {
                Location = new System.Drawing.Point(310, 15),
                Text = "Категория:",
                AutoSize = true,
                Name = "categoryLabel"
            };

            dateLabel = new Label
            {
                Location = new System.Drawing.Point(440, 15),
                Text = "Дата:",
                AutoSize = true,
                Name = "dateLabel"
            };

            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(12, 35),
                Width = 155,
                PlaceholderText = "Название",
                Name = "nameTextBox"
            };

            priceTextBox = new TextBox
            {
                Location = new System.Drawing.Point(180, 35),
                Width = 115,
                PlaceholderText = "Цена",
                Name = "priceTextBox"
            };

            categoryComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(310, 35),
                Width = 115,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "categoryComboBox"
            };
            categoryComboBox.Items.AddRange(new object[] { "Продукты", "Техника", "Одежда", "Прочее" });
            categoryComboBox.SelectedIndex = 0;

            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(440, 35),
                Width = 170,
                Format = DateTimePickerFormat.Short,
                Name = "datePicker"
            };

            addPurchaseButton = new Button
            {
                Location = new System.Drawing.Point(12, 70),
                Text = "Добавить",
                Width = 100,
                Height = 30,
                Name = "addPurchaseButton"
            };
            addPurchaseButton.Click += AddPurchaseButton_Click;

            removePurchaseButton = new Button
            {
                Location = new System.Drawing.Point(125, 70),
                Text = "Удалить",
                Width = 100,
                Height = 30,
                Name = "removePurchaseButton"
            };
            removePurchaseButton.Click += RemovePurchaseButton_Click;

            filterLabel = new Label
            {
                Location = new System.Drawing.Point(250, 75),
                Text = "Фильтр по категории:",
                AutoSize = true,
                Name = "filterLabel"
            };

            categoryFilterComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(400, 72),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "categoryFilterComboBox"
            };
            categoryFilterComboBox.Items.AddRange(new object[] { "Все категории", "Продукты", "Техника", "Одежда", "Прочее" });
            categoryFilterComboBox.SelectedIndex = 0;

            filterButton = new Button
            {
                Location = new System.Drawing.Point(530, 70),
                Text = "Фильтровать",
                Width = 90,
                Height = 30,
                Name = "filterButton"
            };
            filterButton.Click += FilterButton_Click;

            purchasesListBox = new ListBox
            {
                Location = new System.Drawing.Point(12, 110),
                Width = 608,
                Height = 380,
                Name = "purchasesListBox",
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            #endregion
            #region Analyze
            var analysisGroup = new GroupBox
            {
                Text = "Анализ расходов",
                Location = new System.Drawing.Point(12, 500),
                Width = 608,
                Height = 190,
                Name = "analysisGroup",
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };

            var btnByCategory = new Button
            {
                Text = "По категориям",
                Location = new System.Drawing.Point(10, 25),
                Width = 110,
                Height = 25,
                Name = "btnByCategory"
            };
            btnByCategory.Click += BtnByCategory_Click;

            var btnByPeriod = new Button
            {
                Text = "За период",
                Location = new System.Drawing.Point(130, 25),
                Width = 110,
                Height = 25,
                Name = "btnByPeriod"
            };
            btnByPeriod.Click += BtnByPeriod_Click;

            var btnExport = new Button
            {
                Text = "Экспорт в CSV",
                Location = new System.Drawing.Point(250, 25),
                Width = 110,
                Height = 25,
                Name = "btnExport"
            };
            btnExport.Click += BtnExport_Click;

            var periodLabel = new Label
            {
                Text = "Период:",
                Location = new System.Drawing.Point(10, 60),
                AutoSize = true,
                Name = "periodLabel"
            };

            dateStart = new DateTimePicker
            {
                Location = new System.Drawing.Point(80, 57),
                Width = 120,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1),
                Name = "dateStart"
            };

            dateEnd = new DateTimePicker
            {
                Location = new System.Drawing.Point(210, 57),
                Width = 120,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Name = "dateEnd"
            };
            resultLabel = new Label
            {
                Location = new System.Drawing.Point(10, 90),
                Width = 580,
                Height = 90,
                AutoSize = false,
                ForeColor = System.Drawing.Color.DarkBlue,
                Name = "resultLabel",
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic)
            };

            // Диаграмма
            expenseChart = new Chart
            {
                Location = new System.Drawing.Point(12, 660), 
                Width = 608,
                Height = 280,
                Visible = false  
            };

            var chartArea = new ChartArea("MainArea");
            expenseChart.ChartAreas.Add(chartArea);

            var series = new Series("Categories")
            {
                ChartType = SeriesChartType.Pie,
                LegendText = "#VALX",
                XValueMember = "Category",
                YValueMembers = "Amount",
                Label = "#VALX: #VAL{C} руб.",
                LabelAngle = -45,
                IsValueShownAsLabel = true
            };
            expenseChart.Series.Add(series);

            var legend = new Legend("Legend")
            {
                Docking = Docking.Right,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            expenseChart.Legends.Add(legend);

            expenseChart.Titles.Add("Расходы по категориям");

            var btnToggleChart = new Button
            {
                Text = "Показать график",
                Location = new System.Drawing.Point(370, 25),
                Width = 110,
                Height = 25,
                Name = "btnToggleChart"
            };
            btnToggleChart.Click += BtnToggleChart_Click;
            analysisGroup.Controls.Add(btnToggleChart);
            #endregion
            #region ObjectsForm
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
            this.Controls.Add(analysisGroup);
            this.Controls.Add(expenseChart);
            analysisGroup.Controls.Add(btnByCategory);
            analysisGroup.Controls.Add(btnByPeriod);
            analysisGroup.Controls.Add(btnExport);
            analysisGroup.Controls.Add(periodLabel);
            analysisGroup.Controls.Add(dateStart);
            analysisGroup.Controls.Add(dateEnd);
            analysisGroup.Controls.Add(resultLabel);

            purchaseManager = new PurchaseManager();
            currentDisplayedPurchases = new List<Purchase>();
            UpdatePurchasesList();
            #endregion


        }
        #region EventsMain
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
            currentDisplayedPurchases.Clear();
            foreach (var purchase in filteredPurchases)
            {
                purchasesListBox.Items.Add($"{purchase.Name} - {purchase.Price} руб. ({purchase.Category})");
                currentDisplayedPurchases.Add(purchase);
            }
        }
        #endregion
        #region EventsAnalyze
        private void BtnByCategory_Click(object sender, EventArgs e)
        {
            var analyzer = new ExpenseAnalyzer(purchaseManager.Purchases);
            var stats = analyzer.GetTotalsByCategory();

            if (stats.Count == 0)
            {
                resultLabel.Text = "Нет данных для анализа.";
                return;
            }

            var report = new StringBuilder("Расходы по категориям:\n");
            foreach (var item in stats.OrderByDescending(x => x.Value))
            {
                report.AppendLine($" {item.Key}: {item.Value} руб.");
            }
            report.AppendLine($"Итого: {stats.Values.Sum()} руб.");
            resultLabel.Text = report.ToString();
            UpdateCategoryChart(stats);
        }

        private void UpdateCategoryChart(Dictionary<string, decimal> stats)
        {
            var series = expenseChart.Series["Categories"];
            series.Points.Clear();

            // Добавляем данные, сортируя по убыванию
            foreach (var item in stats.OrderByDescending(x => x.Value))
            {
                series.Points.AddXY(item.Key, (double)item.Value);
            }

            var colors = new[] {
        System.Drawing.Color.SkyBlue,
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.LightCoral,
        System.Drawing.Color.Khaki,
        System.Drawing.Color.Plum
    };
            for (int i = 0; i < series.Points.Count; i++)
            {
                series.Points[i].Color = colors[i % colors.Length];
            }

            expenseChart.Visible = true;
        }

        private void BtnByPeriod_Click(object sender, EventArgs e)
        {
            var start = dateStart.Value;
            var end = dateEnd.Value;

            if (start > end)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var analyzer = new ExpenseAnalyzer(purchaseManager.Purchases);
            var total = analyzer.GetTotalByPeriod(start, end);
            var byCategory = analyzer.GetTotalsByCategoryForPeriod(start, end);

            var report = new StringBuilder($"Расходы с {start:dd.MM} по {end:dd.MM}:\n");
            if (byCategory.Count == 0)
            {
                report.AppendLine("Нет покупок за выбранный период");
            }
            else
            {
                foreach (var item in byCategory.OrderByDescending(x => x.Value))
                {
                    report.AppendLine($"{item.Key}: {item.Value} руб.");
                }
            }
            report.AppendLine($"Итого за период: {total} руб.");
            resultLabel.Text = report.ToString();
        }
        private void BtnToggleChart_Click(object sender, EventArgs e)
        {
            expenseChart.Visible = !expenseChart.Visible;
            resultLabel.Visible = !expenseChart.Visible;

            var btn = (Button)sender;
            btn.Text = expenseChart.Visible ? "Показать текст" : "Показать график";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV файлы|*.csv|Все файлы|*.*",
                FileName = $"expenses_{DateTime.Now:yyyy-MM-dd}.csv",
                Title = "Экспорт расходов"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var analyzer = new ExpenseAnalyzer(purchaseManager.Purchases);
                    var csv = analyzer.ExportToCsv();
                    System.IO.File.WriteAllText(saveDialog.FileName, csv, Encoding.UTF8);
                    MessageBox.Show($"Данные экспортированы в:\n{saveDialog.FileName}", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
#endregion

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PurchaseForm());
        }
    }
}