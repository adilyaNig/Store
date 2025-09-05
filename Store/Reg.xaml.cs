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
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Page
    {
        public Reg()
        {
            InitializeComponent();
        }

        private void Reg_Button_Click(object sender, RoutedEventArgs e)
        {
            // Сбор данных из формы
            string name = nameTb.Text.Trim();
            string fam = famTb.Text.Trim();
            string number = numberTb.Text.Trim();
            string email = emailTb.Text.Trim();
            string login = loginTb.Text.Trim();
            string password = passwordTb.Password.Trim();

            // Базовая проверка данных
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(fam) ||
                string.IsNullOrWhiteSpace(number) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавляем пользователя в базу данных
            using (var context = new ShopEntities())
            {
                var existingUser = context.UserAccount.FirstOrDefault(u => u.login == login);
                if (existingUser != null)
                {
                    MessageBox.Show("Такой логин уже занят. Выберите другой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем нового пользователя
                var newUser = new UserAccount
                {
                    name = name,
                    fam = fam,
                    number = number,
                    email = email,
                    login = login,
                    password = password,
                    isDelete = false
                };

                context.UserAccount.Add(newUser);
                context.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack(); // Возврат на предыдущую страницу
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }
    }


}
