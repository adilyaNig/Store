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
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public static UserAccount currentUser; // Сделаем его статическим, чтобы был доступен в любом месте приложения

        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTb.Text.Trim();
            string password = passwordTb.Password.Trim();

            var result = CheckCredentials(login, password);

            if (result != null)
            {
                currentUser = result; // Сохраняем пользователя в статическое поле
                isAuth.IsAuthorized = true;
                NavigationService.Navigate(new GlavMenu());
            }
            else
            {
                MessageBox.Show("Логин или пароль неверный");
            }
        }

        private UserAccount CheckCredentials(string login, string password)
        {
            using (var context = new ShopEntities())
            {
                var userAccountQuery = from v in context.UserAccount
                                       where v.login == login && v.password == password
                                       select v;

                return userAccountQuery.FirstOrDefault();
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Reg());
        }

    }
}
