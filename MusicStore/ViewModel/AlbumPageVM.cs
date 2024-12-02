using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
namespace MusicStore.ViewModel
{
    public class AlbumPageVM : BaseViewModel
    {
        private List<Track> _tracks;
        private string _albumName;
        private BitmapImage _coverImage;

        private MusicStoreContext context = new MusicStoreContext();
        public BitmapImage CoverImage
        {
            get => _coverImage;
            set
            {
                _coverImage = value;
                OnPropertyChanged();
            }
        }
        public string AlbumName
        {
            get => _albumName;
            set
            {
                _albumName = value;
                OnPropertyChanged();
            }
        }
        public List<Track> ListTrack
        {
            get => _tracks;
            set
            {
                _tracks = value;
                OnPropertyChanged();
            }
        }
        public ICommand ChoosePlaylistCommand { get; set; }
        public AlbumPageVM(MainViewModel mvm, Album a)
        {
            _albumName = a.Title;
            LoadImageFromProject(a);
            var context = new MusicStoreContext();
            ListTrack = context.Tracks.Where(track => track.AlbumId == a.AlbumId).ToList();
            ChoosePlaylistCommand = new RelayCommand<TextBlock>((p) => { return checkPlaylist(mvm); }, (p) => { OpenPlaylist(p, mvm); });

        }
        private void LoadImageFromProject(Album a)
        {
            if (File.Exists(a.CoverImage))
            {
                Uri imageUri = new Uri(a.CoverImage, UriKind.RelativeOrAbsolute);
                CoverImage = new BitmapImage(imageUri);
            }
            else
            {
                MessageBox.Show("Image file not found: " + a.CoverImage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void OpenPlaylist(TextBlock block, MainViewModel mvm)
        {
            if (mvm.IsLoggedIn == false)
            {
                MessageBox.Show("Bạn cần đăng nhập để thực hiện chức năng này");
                return;
            }
            var context = new MusicStoreContext();
            var Playlists = context.Playlists
            .Where(p => p.CustomerId == mvm.User.CustomerId)
            .ToList();
            if (Playlists.Count <= 0)
            {
                MessageBox.Show("Không có playlist");
            }
            
            int id = int.Parse(block.Text);
            Track current = context.Tracks.Find(id);
            var pl = new ChoosePlaylist(current,mvm);
            pl.ShowDialog();
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
    }
}
