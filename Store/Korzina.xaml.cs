using Store.DBConnection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для Korzina.xaml
    /// </summary>
    public partial class Korzina : Page, INotifyPropertyChanged
    {

        public ObservableCollection<Store.DBConnection.Toy> SelectedToys { get; set; }
    = new ObservableCollection<Store.DBConnection.Toy>();

        // Свойство для итоговой суммы
        private decimal _totalSum;
        public decimal TotalSum
        {
            get => _totalSum;
            set
            {
                _totalSum = value;
                OnPropertyChanged(nameof(TotalSum));
            }
        }

        public Korzina()
        {
            InitializeComponent();
            LoadSelectedToys(); // Загружаем товары с IsSelected = true
            DataContext = this;
        }
      
        // Метод для загрузки и обновления списка товаров
        internal void LoadSelectedToys()
        {
            using (var context = new ShopEntities())
            {
                var selected = context.Toy.Where(t => t.IsSelected == true).ToList();
                SelectedToys.Clear();
                foreach (var item in selected)
                {
                    SelectedToys.Add(item);
                }
                ReCalculateTotalSum();
            }
        }

        
        private void OplataButton_Click(object sender, RoutedEventArgs e)
        {
            if (TotalSum == 0)
            {
                MessageBox.Show("У вас пустая корзина");
            }
            else
            {

                var currentUser = AuthorizationPage.currentUser;

                if (currentUser != null)
                {
                    Oplata oplata = new Oplata(currentUser, SelectedToys, this);

                    // Показываем окно оплаты и ждем, когда оно закроется
                    if (oplata.ShowDialog() == true) // Предполагается, что окно оплаты возвращает true при успешной оплате
                    {
                        // Очистка корзины после успешной оплаты

                        SelectedToys.Clear();
                        TotalSum = 0;
                        MessageBox.Show("Оплата прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Необходимо войти в систему для продолжения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Вспомогательный метод для пересчета итоговой суммы
        public void ReCalculateTotalSum()
        {
            TotalSum = SelectedToys.Sum(toy => toy.price.GetValueOrDefault());
        }

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Обработчик события удаления товара
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.CommandParameter is Store.DBConnection.Toy toy)
            {
                using (var context = new ShopEntities())
                {
                    // Получаем текущий товар из контекста данных
                    var selectedToyEntity = context.Toy.SingleOrDefault(t => t.idToy == toy.idToy);
                    if (selectedToyEntity != null)
                    {
                        // Устанавливаем IsSelected = false
                        selectedToyEntity.IsSelected = false;

                        // Сохраняем изменения в базе данных
                        context.SaveChanges();

                        // Удаляем товар из коллекции
                        SelectedToys.Remove(toy);

                        // Пересчитываем итоговую сумму
                        ReCalculateTotalSum();
                    }
                }
            }
        }

    }
}


