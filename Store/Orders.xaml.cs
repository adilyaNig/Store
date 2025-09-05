using Store.DBConnection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
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
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Store.DBConnection.Order> UserOrders { get; set; } = new ObservableCollection<Store.DBConnection.Order>();
        public ObservableCollection<Store.DBConnection.OrderDetails> SelectedOrderDetails { get; set; } = new ObservableCollection<Store.DBConnection.OrderDetails>();

        private Store.DBConnection.Order _selectedOrder;
        public Store.DBConnection.Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
                LoadOrderDetails(value?.idOrder); // Загружаем детали при смене заказа
            }
        }

        public Orders(int userId)
        {
            InitializeComponent();
            DataContext = this;
            LoadUserOrders(userId);
        }

        private async void LoadUserOrders(int userId)
        {
            using (var context = new ShopEntities())
            {
                var orders = await context.Order
                    .Where(o => o.idUser == userId && !(o.isDelete ?? false))
                    .ToListAsync();

                UserOrders.Clear();
                foreach (var order in orders)
                {
                    UserOrders.Add(order);
                }
            }
        }

        private async void LoadOrderDetails(int? orderId)
        {
            if (orderId == null) return;

            using (var context = new ShopEntities())
            {
                var details = await context.OrderDetails
                    .Where(d => d.idOrder == orderId)
                    .Include(d => d.Toy) // Подгружаем информацию о товаре
                    .ToListAsync();

                SelectedOrderDetails.Clear();
                foreach (var detail in details)
                {
                    SelectedOrderDetails.Add(detail);
                }
            }
        }
        private void ordersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem is Store.DBConnection.Order selectedOrder)
            {
                SelectedOrder = selectedOrder;
                detailsGrid.Visibility = Visibility.Visible; // Показываем панель с деталями
            }
            else
            {
                detailsGrid.Visibility = Visibility.Collapsed; // Скрываем панель с деталями
            }
        }

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
