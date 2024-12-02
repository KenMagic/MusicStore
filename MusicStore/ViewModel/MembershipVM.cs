using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using MusicStore.Models;

namespace MusicStore.ViewModel
{
    public class MembershipPlan
    {
        public string PlanName { get; set; }
        public decimal Amount { get; set; }
    }

    public class MembershipVM : BaseViewModel
    {
        private ObservableCollection<MembershipPlan> _membershipPlans;
        private MembershipPlan _selectedPlan;
        private readonly MusicStoreContext _context;

        public ObservableCollection<MembershipPlan> MembershipPlans
        {
            get => _membershipPlans;
            set
            {
                _membershipPlans = value;
                OnPropertyChanged();
            }
        }

        public MembershipPlan SelectedPlan
        {
            get => _selectedPlan;
            set
            {
                _selectedPlan = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmCommand { get; }

        public MembershipVM(MainViewModel mvm)
        {
            _context = new MusicStoreContext();
            LoadMembershipPlans();
            ConfirmCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ConfirmSelection(mvm); });
        }

        private void LoadMembershipPlans()
        {
            // Loading predefined plans, but these could also be loaded from the database
            MembershipPlans = new ObservableCollection<MembershipPlan>
            {
                new MembershipPlan { PlanName = "Monthly", Amount = 9.99m },
                new MembershipPlan { PlanName = "Annual", Amount = 99.99m }
            };
        }
        Membership CheckCurrentMembership(User u)
        {
            var context = new MusicStoreContext();
            var m = context.Memberships
                .Where(m => m.CustomerId == u.CustomerId)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefault();
            return m;
        }
        private void ConfirmSelection(MainViewModel mvm)
        {
            Membership membership = CheckCurrentMembership(mvm.User);
            if (membership != null && membership.Status == "Active")
            {
                MessageBox.Show($"You already have an active membership. Your current plan is {membership.MembershipPlan}.");
                return;
            }
            if (SelectedPlan != null)
            {
                var context = new MusicStoreContext();
                Membership m = new Membership
                {
                    CustomerId = mvm.User.CustomerId,
                    MembershipPlan = SelectedPlan.PlanName,
                    Amount = SelectedPlan.Amount,
                    StartDate = global::System.DateTime.Now,
                    Status = "Active"
                };
                if (m.MembershipPlan == "Monthly")
                {
                    m.EndDate = m.StartDate?.AddMonths(1);
                }
                else
                {
                    m.EndDate = m.StartDate?.AddYears(1);
                }
                context.Memberships.Add(m);
                context.SaveChanges();
                MessageBox.Show($"Selected Plan: {SelectedPlan.PlanName}, Amount: {SelectedPlan.Amount:C}");
                mvm.ChangePage(new MyProfile(mvm));
            }
            else
            {
                MessageBox.Show("Please select a membership plan.");
            }
        }
    }
}
