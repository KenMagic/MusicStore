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
using MusicStore.Models;
namespace MusicStore
{
    /// <summary>
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class Password : Page
    {
        public User current {  get; set; }
        public Password(User current)
        {
            InitializeComponent();
            this.current = current;
            passwordBox.Password = current.Password;
        }

        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RegisterPage(current));
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = passwordBox.Password;

            // Check conditions for password validity
            bool isValidLength = password.Length >= 6 && password.Length <= 16;
            bool hasNoSpace = !password.Contains(" ");

            // Update checkboxes based on conditions
            checkBoxLength.IsChecked = isValidLength;
            checkBoxNoSpace.IsChecked = hasNoSpace;

            // Enable the Next button if both conditions are met
            Nextbtn.IsEnabled = isValidLength && hasNoSpace;
        }

        private void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            current.Password = passwordBox.Password;
            this.NavigationService.Navigate(new UserProfileRegis(current));
        }
    }
}
