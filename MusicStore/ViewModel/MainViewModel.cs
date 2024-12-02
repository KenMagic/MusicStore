using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MusicStore.Models;
using System.Windows.Navigation;
using System.Windows.Input;
using MusicStore.View;
using System.Windows.Controls;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.IdentityModel.Tokens;
namespace MusicStore.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private bool _isLoggedIn;
        private bool _isAdmin;
        private User _currentUser;
        private object _currentPage;
        private ObservableCollection<Track> _currentTrack;
        private string _searchQuery;
        private DispatcherTimer _searchTimer;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }
        public User User
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Track> CurrentTrack
        {
            get => _currentTrack;
            set
            {
                _currentTrack = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand MyPlayListCommand { get; set; }
        public ICommand MyProfileCommand { get; set; }

        public ICommand HomeCommand { get; set; }
        public ICommand MusicListener { get; set; }

        public ICommand SearchCommand { get; set; }

        public ICommand AppManageCommand { get; set; }    
        public ICommand SearchBarChanged { get; set; }
        // Constructor
        public MainViewModel()
        {
            IsLoggedIn = false;
            _currentPage = new HomePage(this);
            LoginCommand = new RelayCommand<object>((p) => { return true; }, LoginWindowOpen);
            LogOutCommand = new RelayCommand<object>((p) => { return true; }, LogOut);
            HomeCommand = new RelayCommand<object>((p) => { return true; }, (p) => { this.ChangePage(new HomePage(this)); });
            RegisterCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                var register = new Register();
                register.ShowDialog();

            });
            MyPlayListCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                this.ChangePage(new MyPlaylist(this));
            });
            MyProfileCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                this.ChangePage(new MyProfile(this));
            });
            MusicListener = new RelayCommand<object>((p) => { return IsLoggedIn == true; }, (p) =>
            {
                var _context = new MusicStoreContext();
                var membership = _context.Memberships
                .Where(m => m.CustomerId == this.User.CustomerId)
                .Where(m => m.Status == "Active")
                .FirstOrDefault();
                if (membership == null)
                {
                    MessageBox.Show("You need to have an active membership to play a playlist");
                    return;
                }
                this.ChangePage(new MusicListener(this));
            });
            _searchTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                this.ChangePage(new HomePage(this));
                
            });
            AppManageCommand = new RelayCommand<object>((p) => { return IsAdmin; }, (p) =>
            {
                this.ChangePage(new AppManage());
            });
            SearchBarChanged = new RelayCommand<TextBox>((p) => { return true; }, (p) =>
            {
                SearchQuery = p.Text;
            });
        }



        void LoginWindowOpen(object p)
        {
            var login = new Login(this);
            login.ShowDialog();
        }
        public void ChangePage(object newPage)
        {
            CurrentPage = newPage;
        }
        private void LogOut(object p)
        {
            IsLoggedIn = false;
            User = null;
            CurrentTrack = null;
            IsAdmin = false;
            this.ChangePage(new HomePage(this));
        }
        private void ResetSearchTimer() { _searchTimer.Stop(); _searchTimer.Start(); }

    }
            
}
