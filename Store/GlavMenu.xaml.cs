using Store.DBConnection;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для GlavMenu.xaml
    /// </summary>
    public partial class GlavMenu : Page
    {
        public GlavMenu()
        {
            InitializeComponent();
            menuFr.NavigationService.Navigate(new Toy());
        }

        private void Buy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            menuFr.NavigationService.Navigate(new Korzina());
        }

        private void Acc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            menuFr.NavigationService.Navigate(new Acc(AuthorizationPage.currentUser));
        }

        private void Home_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(menuFr.Content is Toy))
                menuFr.NavigationService.Navigate(new Toy());
        }
    }
}
