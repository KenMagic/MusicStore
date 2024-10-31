using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
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

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    
    public partial class RegisterPage : Page
    {
        public User? current { get; set; }
        private readonly MusicStoreContext _context;
        public RegisterPage()
        {
            InitializeComponent();
            _context = new MusicStoreContext();
        }
        public RegisterPage(User current)
        {
            InitializeComponent();
            this.txtUsername.Text = current.Username;
            _context = new MusicStoreContext();
        }
        private async void Nextbtn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;

            Debug.WriteLine("Button clicked. Checking username...");

            if (string.IsNullOrWhiteSpace(username))
            {
                txtNotification.Visibility = Visibility.Visible;
                txtNotification.Text = "Please enter a username.";
                Debug.WriteLine("Username is empty.");
            }
            else
            {
                txtNotification.Visibility = Visibility.Collapsed;

                try
                {
                    Debug.WriteLine($"Checking if the username '{username}' exists in the database...");

                    var existingUser = await _context.Users
                                                      .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

                    if (existingUser != null)
                    {
                        txtNotification.Visibility = Visibility.Visible;
                        txtNotification.Text = "Username already exists. Please choose a different one.";
                        Debug.WriteLine("Username already exists.");
                    }
                    else
                    {
                        txtNotification.Visibility = Visibility.Collapsed;

                        User current = new User { Username = username };
                        Debug.WriteLine("Navigating to Password page...");

                        this.NavigationService.Navigate(new Password(current));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                    txtNotification.Visibility = Visibility.Visible;
                    txtNotification.Text = $"An error occurred: {ex.Message}";
                }
            }
        }


    }
}
