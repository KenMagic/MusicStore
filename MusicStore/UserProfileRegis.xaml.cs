using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for UserProfileRegis.xaml
    /// </summary>
    public partial class UserProfileRegis : Page
    {
        public User current {  get; set; }
        public UserProfileRegis(User current)
        {
            InitializeComponent();
            this.current = current;
        }

        private async void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            // Clear any previous notifications
            txtNotification.Visibility = Visibility.Collapsed;

            // Validate fields
            bool isValid = true;
            string notificationMessage = string.Empty;

            // Check if name is provided and not just whitespace
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                notificationMessage += "Name is required.\n";
                isValid = false;
            }

            // Check if email is provided and ends with "@gmail.com" if not null
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.EndsWith("@gmail.com"))
            {
                notificationMessage += "If provided, email must end with @gmail.com.\n";
                isValid = false;
            }

            // Check if phone contains only digits or is empty
            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !txtPhone.Text.All(char.IsDigit))
            {
                notificationMessage += "Phone number must contain only digits.\n";
                isValid = false;
            }

            // Address can be empty, no additional checks needed

            // If not valid, show notification
            if (!isValid)
            {
                txtNotification.Text = notificationMessage;
                txtNotification.Visibility = Visibility.Visible;
            }
            else
            {
                Customer newCustomer = new Customer()
                {
                    Name = txtName.Text,
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                    Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text,
                    Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text
                };
                try
                {
                    using (var context = new MusicStoreContext())
                    {
                        // Add new customer
                        context.Customers.Add(newCustomer);
                        await context.SaveChangesAsync(); // Save to get the new CustomerId

                        // Create and add new user
                        User newUser = new User()
                        {
                            Username = current.Username,
                            Password = current.Password, // Make sure to hash the password
                            CustomerId = newCustomer.CustomerId,
                            CreatedAt = DateTime.Now
                        };

                        context.Users.Add(newUser);
                        await context.SaveChangesAsync();

                        // Navigate to the next page
                        this.NavigationService.Navigate(new Done());
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (consider using a logging library or Debug.WriteLine)
                    txtNotification.Text = $"An error occurred: {ex.Message}";
                    txtNotification.Visibility = Visibility.Visible;
                    if (ex.InnerException != null)
                    {
                        txtNotification.Text += $"\nInner exception: {ex.InnerException.Message}";
                    }

                    // Optionally log the stack trace for further analysis
                    Debug.WriteLine(ex.StackTrace);
                    
                }
            }
            }
        


        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Password(current));
        }
    }
}
