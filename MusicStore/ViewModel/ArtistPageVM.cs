using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicStore.Models;
namespace MusicStore.ViewModel
{
    public class ArtistPageVM : BaseViewModel
    {
        private List<Album> _albums;
        private string _artistName;
        private string _biography;
        public string Biography
        {
            get => _biography;
            set
            {
                _biography = value;
                OnPropertyChanged();
            }
        }
        public string ArtistName
        {
            get => _artistName;
            set
            {
                _artistName = value;
                OnPropertyChanged();
            }
        }
        public List<Album> ListAlbum
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged();
            }
        }
        //AddAlbumCommand
        public ICommand AddAlbumCommand { get; set; }
        public ArtistPageVM(MainViewModel mvm, Artist a)
        {
            _artistName = a.Name;
            Biography = a.Biography;
            var context = new MusicStoreContext();
            ListAlbum = context.Albums.Where(album => album.ArtistId == a.ArtistId).ToList();
            AddAlbumCommand = new RelayCommand<int>((p) => { return checkPlaylist(mvm); ; }, (p) =>
            {
                OpenPlaylist(p, mvm);
            });
        }
        private bool checkPlaylist(MainViewModel mvm)
        {
            var context = new MusicStoreContext();
            var Playlists = context.Playlists
            .Where(p => p.CustomerId == mvm.User.CustomerId)
            .ToList();
            if (Playlists.Count > 0)
            {
                return true;
            }
            return false;
        }
        private void OpenPlaylist(int id, MainViewModel mvm)
        {
            var context = new MusicStoreContext();
            if (mvm.IsLoggedIn == false)
            {
                MessageBox.Show("Bạn cần đăng nhập để thực hiện chức năng này");
                return;
            }
            var Playlists = context.Playlists
            .Where(p => p.CustomerId == mvm.User.CustomerId)
            .ToList();
            if (Playlists.Count <= 0)
            {
                MessageBox.Show("Không có playlist");
            }
            Album current = context.Albums.Find(id);
            var pl = new ChoosePlaylist(current, mvm);
            pl.ShowDialog();
        }


    }
   
    
}
