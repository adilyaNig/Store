using Store.DBConnection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Store
{
    /// <summary>
    /// Логика взаимодействия для Toy.xaml
    /// </summary>
    public partial class Toy : Page
    {
        private bool isFiltering = false;
        public List<Store.DBConnection.Toy> toys { get; set; } = new List<Store.DBConnection.Toy>();
        

        public Toy()
        {
            InitializeComponent();
            toys = DBConnection.Connection.shop.Toy.Where(p => p.isDelete == false).ToList();
            this.DataContext = this;

            filterAgeComboBox.SelectedIndex = 0;
            categoryComboBox.SelectedIndex = 0;
        }
        private IEnumerable<Store.DBConnection.Toy> ApplyFilters()
        {
            var filteredToys = toys.AsEnumerable(); // Начинаем с полного списка

            // Фильтрация по возрасту
            if (filterAgeComboBox.SelectedItem is ComboBoxItem ageItem && ageItem.Tag.ToString() != "all")
            {
                string selectedAge = ageItem.Tag.ToString();

                filteredToys = filteredToys.Where(toy =>
                    !string.IsNullOrWhiteSpace(toy.age) &&
                    toy.age
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim()) // Удаляем пробелы
                        .Contains(selectedAge)
                );
            }

            // Фильтрация по категории
            if (categoryComboBox.SelectedItem is ComboBoxItem categoryItem && categoryItem.Tag.ToString() != "all")
            {
                string category = categoryItem.Tag.ToString();
                filteredToys = filteredToys.Where(toy => toy.category == category);
            }

            // Фильтрация по цене (если нужно)
            if (sortComboBox.SelectedItem is ComboBoxItem sortItem && sortItem.Tag.ToString() != "default")
            {
                string sortOption = sortItem.Tag.ToString();
                switch (sortOption)
                {
                    case "ascending":
                        filteredToys = filteredToys.OrderBy(toy => toy.price);
                        break;
                    case "descending":
                        filteredToys = filteredToys.OrderByDescending(toy => toy.price);
                        break;
                }
            }

            return filteredToys.ToList();
        }
        private void ToysLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isFiltering) // Открываем всплывающее окно только если не происходит фильтрация
            {
                popupDetail.IsOpen = true; // Открываем окно при выборе товара
            }
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            popupDetail.IsOpen = false; // Закрытие всплывающего окна
        }
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isFiltering = true; // Устанавливаем флаг перед изменением фильтра

            ToysLv.ItemsSource = ApplyFilters();

            isFiltering = false; // Сбрасываем флаг после изменения фильтра
        }

        private void FilterAgeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isFiltering = true; // Устанавливаем флаг перед изменением фильтра

            ToysLv.ItemsSource = ApplyFilters();

            isFiltering = false; // Сбрасываем флаг после изменения фильтра
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isFiltering = true; // Устанавливаем флаг перед изменением фильтра

            ToysLv.ItemsSource = ApplyFilters();

            isFiltering = false; // Сбрасываем флаг после изменения фильтра
        }
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.CommandParameter is Store.DBConnection.Toy toy)
            {
                if (!toy.isDelete.HasValue || !toy.isDelete.Value)
                {
                    using (var context = new ShopEntities())
                    {
                        var selectedToy = context.Toy.Find(toy.idToy);
                        if (selectedToy != null)
                        {
                            selectedToy.IsSelected = true;
                            context.SaveChanges(); // Сохраняем изменения
                        }
                    }

                    MessageBox.Show("Товар добавлен в корзину!");
                }
                else
                {
                    MessageBox.Show("Данный товар удалён и недоступен для покупки.");
                }
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = searchTextBox.Text.Trim(); // Получаем текст из TextBox

            if (string.IsNullOrEmpty(search)) // Проверяем, пуст ли ввод
                ToysLv.ItemsSource = toys.ToList(); // Если пусто, показываем все записи
            else
                // Фильтруем по названию товара, игнорируя регистр
                ToysLv.ItemsSource = toys
                    .Where(i => i.name != null && i.name.ToLower().Contains(search.ToLower()))
                    .ToList();
        }

        private void Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Visibility = Visibility.Visible;
            searchTextBox.Focus();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Восстанавливаем исходный список
            ToysLv.ItemsSource = toys.ToList();

            // Сбрасываем выбор в комбобоксах
            sortComboBox.SelectedIndex = -1;
            filterAgeComboBox.SelectedIndex = 0; // 'Все'
            categoryComboBox.SelectedIndex = 0; // 'Все'

            // Очищаем поиск
            searchTextBox.Text = "";
        }
    }



}

