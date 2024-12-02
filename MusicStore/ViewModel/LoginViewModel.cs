using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Controls;
using MusicStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
namespace MusicStore.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {

        public bool IsLogin { get; set; }
        private MusicStoreContext context = new MusicStoreContext();
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }
        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public LoginViewModel(MainViewModel mvm)
        {
            IsLogin = false;
            UserName = "";
            Password = "";
            LoginCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { Login(p,mvm); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }

        public void Login(Window p, MainViewModel mvm)
        {
            if (p == null)
            {
                return;
            }
            var context = new MusicStoreContext();
            var Acc = context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Username == UserName);

            if (Acc != null && Acc.Password == Password)
            {
                mvm.User = Acc;
                if(HaveHistory(Acc))
                {
                    var history = context.ListeningHistories
                        .Include(u => u.Customer)
                        .FirstOrDefault(l => l.CustomerId == Acc.Customer.CustomerId);

                    if (history == null || history.Playlist == null)
                    {
                        mvm.CurrentTrack = null;
                    }

                    var playlist = history.Playlist;
                    var tracks = playlist.Tracks.ToList();
                    mvm.CurrentTrack = new ObservableCollection<Track>(tracks);

                }
                CheckMembershipStatus(Acc);
                mvm.IsAdmin = CheckIfAdmin(Acc);
                mvm.IsLoggedIn = true;
                p.Close();
                return;
            }
            IsLogin = false;
            MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
        }
        bool HaveHistory(User a)
        {

            if (a?.Customer == null)
            {
                return false;
            }

            var history = context.ListeningHistories
                                   .FirstOrDefault(l => l.CustomerId == a.Customer.CustomerId);

            return history != null;
        }
        private void CheckMembershipStatus(User a)
        {
            if (a == null)
            {
                return;
            }

            var membership = context.Memberships
                .Where(m => m.CustomerId == a.CustomerId)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefault();

            if (membership == null)
            {
                MessageBox.Show("No membership found for the customer.");
                return;
            }

            if (membership.EndDate < DateTime.Now)
            {
                membership.Status = "Expired";
                context.SaveChanges();
                MessageBox.Show("Your membership has expired.");
            }
            else 
            { 
                MessageBox.Show($"Your membership is active until {membership.EndDate}.");
            }
        }

        private bool CheckIfAdmin(User a)
        {
            if (a == null) return false;
            if (a.Roles == null || !a.Roles.Any())
            {
                return false;
            }
            return true;
        }

    }
}
