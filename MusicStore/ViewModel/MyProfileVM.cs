using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MusicStore.ViewModel
{
    public class MyProfileVM : BaseViewModel
    {
        private User _user;
        private string _member;
        private string _role;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        public string UserRole
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
            }
        }
        public string Membership
        {
            get => _member;
            set
            {
                _member = value;
                OnPropertyChanged();
            }
        }
        public ICommand MembershipCommand { get; set; }
        public MyProfileVM(MainViewModel mvm)
        {
            var context = new MusicStoreContext();
            User = mvm.User;
            Membership = CheckMembershipStatus(User);
            User.Customer = context.Customers.Find(User.CustomerId);
            var UserTemp = context.Users
                  .Include(u => u.Roles) 
                  .FirstOrDefault(u => u.UserId == User.UserId);
            UserRole = GetUserRole(UserTemp);
            MembershipCommand = new RelayCommand<object>((p) => { return true; }, (p) => { OpenMembership(mvm); });
        }

        private void OpenMembership(MainViewModel mvm)
        {
            var membership = new MembershipWindow(mvm);
            //
            membership.ShowDialog();
        }

        public string GetUserRole(User user)
        {
            if (user?.Roles != null && user.Roles.Any())
            {
                return user.Roles.First().RoleName; 
            }
            else
            {
                // If no roles found, treat as "Customer"
                return "Customer";
            }
        }
        private string CheckMembershipStatus(User a)
        {
            var context = new MusicStoreContext();
            if (a == null)
            {
                return "No membership";
            }

            var membership = context.Memberships
                .Where(m => m.CustomerId == a.CustomerId)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefault();

            if (membership == null)
            {
                return "No membership";
            }

            if (membership.EndDate < DateTime.Now)
            {
                membership.Status = "Expired";
                context.SaveChanges();
                return "Your membership has expired.";
            }
            else
            {
                return $"Your membership is active until {membership.EndDate}.";
            }
        }

    }
}
