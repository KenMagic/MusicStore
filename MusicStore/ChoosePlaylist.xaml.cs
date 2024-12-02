using MusicStore.ViewModel;
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
using System.Windows.Shapes;
using MusicStore.Models;
namespace MusicStore
{
    /// <summary>
    /// Interaction logic for ChoosePlaylist.xaml
    /// </summary>
    public partial class ChoosePlaylist : Window
    {
        public ChoosePlaylist(object c, MainViewModel mvm)
        {
            InitializeComponent();
            DataContext = new PlaylistViewModel(c, mvm);
        }
    }
}
