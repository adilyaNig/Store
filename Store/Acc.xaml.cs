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
    /// Логика взаимодействия для Acc.xaml
    /// </summary>
    public partial class Acc : Page
    {
        public Acc(UserAccount user)
        {
            InitializeComponent();
            fioTb.Text = $"Фамилия: {user.fam}";
            nameTb.Text = $"Имя: {user.name}";
            emailTb.Text = $"Email: {user.email}";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Orders(AuthorizationPage.currentUser.idUser));
        }
    }
}
