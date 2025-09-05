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
using System.Windows.Shapes;

namespace Store
{
    /// <summary>
    /// Логика взаимодействия для Oplata.xaml
    /// </summary>
    public partial class Oplata : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Store.DBConnection.Toy> SelectedToys { get; set; }
        private readonly UserAccount _currentUser;
        private readonly Korzina _korzina;
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

        public Oplata(UserAccount currentUser, ObservableCollection<Store.DBConnection.Toy> selectedToys, Korzina korzina)
        {
            InitializeComponent();
            _currentUser = currentUser;
            SelectedToys = selectedToys; // Присваиваем переданную коллекцию
            _korzina = korzina;
            fioTb.Text = $"{_currentUser.fam} {_currentUser.name}";
            numberTb.Text = _currentUser.number;
            emailTb.Text = _currentUser.email;
            TotalSum = CalculateTotalSum();
        }

        private async void ConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(addressDelivery.Text))
            {
                MessageBox.Show("Укажите адрес доставки");
                return;
            }

            if (string.IsNullOrEmpty(cardNumber.Text))
            {
                MessageBox.Show("Укажите номер вашей кредитной карты");
                return;
            }

            using (var context = new ShopEntities())
            {
                // Создаем новый заказ
                var order = new Order
                {
                    idUser = _currentUser.idUser,
                    orderDate = DateTime.Now.Date,
                    deliveryAddress = addressDelivery.Text,
                    totalPrice = _korzina.TotalSum,
                    isDelete = false
                };
                context.Order.Add(order);
                await context.SaveChangesAsync(); // Сохраняем заказ, чтобы получить idOrder

                // Добавляем детали заказа
                foreach (var toy in SelectedToys)
                {
                    var detail = new OrderDetails
                    {
                        idOrder = order.idOrder,
                        idToy = toy.idToy,
                        quantity = 1,
                        subTotal = toy.price.GetValueOrDefault()
                    };
                    context.OrderDetails.Add(detail);
                }

                // Обновляем статус игрушек (необязательно, если это только визуальная логика)
                foreach (var toy in SelectedToys)
                {
                    var toyInDb = context.Toy.SingleOrDefault(t => t.idToy == toy.idToy);
                    if (toyInDb != null)
                    {
                        toyInDb.IsSelected = false;
                    }
                }

                await context.SaveChangesAsync(); // Сохраняем OrderDetails и обновление игрушек

                // Очищаем корзину
                _korzina.LoadSelectedToys();
                _korzina.TotalSum = 0;
                _korzina.OnPropertyChanged(nameof(Korzina.TotalSum));

                MessageBox.Show("Ваш заказ успешно оформлен! Спасибо за покупку.");
                this.Close();
            }
        }

        private void LoadSelectedToys()
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
        private void ReCalculateTotalSum()
        {
            TotalSum = SelectedToys.Sum(toy => toy.price.GetValueOrDefault());
        }
        private async Task CreateAndSaveOrder()
        {
            using (var context = new ShopEntities())
            {
                // Создаем новый заказ
                var order = new Order
                {
                    idUser = _currentUser.idUser,
                    orderDate = DateTime.Now.Date,
                    deliveryAddress = addressDelivery.Text,
                    totalPrice = _korzina.TotalSum,
                    isDelete = false
                };
                context.Order.Add(order);

                // Предварительно сохраняем заказ, чтобы получить корректный idOrder
                await context.SaveChangesAsync();

                // Формируем детали заказа
                foreach (var toy in SelectedToys)
                {
                    var detail = new OrderDetails
                    {
                        idOrder = order.idOrder, // Гарантируется корректный idOrder
                        idToy = toy.idToy,
                        quantity = 1,
                        subTotal = toy.price.GetValueOrDefault() * 1
                    };
                    context.OrderDetails.Add(detail);
                }

                // Теперь сохраняем все изменения (заказ + детали заказа)
                await context.SaveChangesAsync();

                // Далее делаем дополнительные действия (например, сброс корзины)
                foreach (var toy in SelectedToys)
                {
                    toy.IsSelected = false;
                }
                context.SaveChanges(); // Или можешь объединить этот вызов с предыдущими действиями

                _korzina.LoadSelectedToys();
                _korzina.TotalSum = 0;
                _korzina.OnPropertyChanged(nameof(Korzina.TotalSum));

                MessageBox.Show("Ваш заказ успешно оформлен! Спасибо за покупку.");
                this.Close();
            }
        }

        private decimal CalculateTotalSum()
        {
            return SelectedToys.Sum(toy => toy.price.GetValueOrDefault());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

