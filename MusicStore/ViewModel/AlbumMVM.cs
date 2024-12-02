using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MusicStore.ViewModel
{
    public class AlbumMVM : BaseViewModel
    {
        private string _searchText;
        private List<Album> _albums;
        private Album _selectedAlbum;
        private string _albumTitle;
        private Artist _albumArtist;
        private string _albumCover;
        private MusicStoreContext context = new MusicStoreContext();
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchAlbums();
            }
        }
        public List<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged();
            }
        }
        public Album SelectedAlbum
        {
            get => _selectedAlbum;
            set
            {
                _selectedAlbum = value;
                OnPropertyChanged();
                if (value != null)
                {
                    CurrentTitle = value.Title;
                    SelectedArtist = context.Artists.Find(value.ArtistId);
                    CurrentCover = value.CoverImage;
                }
            }
        }
        public string CurrentTitle
        {
            get => _albumTitle;
            set
            {
                _albumTitle = value;
                OnPropertyChanged();
            }
        }
        public Artist SelectedArtist
        {
            get => _albumArtist;
            set
            {
                _albumArtist = value;
                OnPropertyChanged();
            }
        }
        public string CurrentCover
        {
            get => _albumCover;
            set
            {
                _albumCover = value;
                OnPropertyChanged();
            }
        }
        public void SearchAlbums()
        {
            Albums = context.Albums
                .Include(a => a.Artist)
                .Where(a => a.Title.Contains(SearchText)).ToList();
        }
        public ICommand AddAlbumCommand { get; set; }
        public ICommand UpdateAlbumCommand { get; set; }
        public ICommand DeleteAlbumCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ICommand BrowseCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public AlbumMVM()
        {
            Albums = context.Albums.Include(a => a.Artist).ToList();
            AddAlbumCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SelectedArtist == null)
                {
                    MessageBox.Show("Please select an artist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (CurrentTitle == "")
                {
                    MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var context = new MusicStoreContext();
                var album = new Album()
                {
                    Title = CurrentTitle,
                    ArtistId = SelectedArtist.ArtistId,
                    CoverImage = CurrentCover,
                    ReleaseDate = DateOnly.FromDateTime(DateTime.Now)
                };
                context.Albums.Add(album);
                context.SaveChanges();
                Albums = context.Albums.Include(a => a.Artist).ToList();
            });
            UpdateAlbumCommand = new RelayCommand<object>((p) => { return SelectedAlbum != null; }, (p) =>
            {
                if (SelectedArtist == null)
                {
                    MessageBox.Show("Please select an artist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (CurrentTitle == "")
                {
                    MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Album current = context.Albums.Find(SelectedAlbum.AlbumId);
                current.Title = CurrentTitle;
                current.Artist = SelectedArtist;
                current.CoverImage = CurrentCover;
                context.SaveChanges();
                Albums = context.Albums.Include(a => a.Artist).ToList();
            });
            DeleteAlbumCommand = new RelayCommand<object>((p) => { return SelectedAlbum != null; }, (p) =>
            {
                var context = new MusicStoreContext();
                var album = context.Albums.Find(SelectedAlbum.AlbumId);
                context.Albums.Remove(album);
                context.SaveChanges();
                Albums = context.Albums.Include(a => a.Artist).ToList();
            });
            RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentTitle = "";
                SelectedArtist = null;
                CurrentCover = "";
                SelectedAlbum = null;
            });
            BrowseCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                    Title = "Select a Cover Image"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string imageDirectory = Path.Combine(projectDirectory, "Image");

                    if (!Directory.Exists(imageDirectory))
                    {
                        Directory.CreateDirectory(imageDirectory);
                    }

                    string newFileName = Path.Combine(imageDirectory, Path.GetFileName(selectedFilePath));

                    try
                    {
                        File.Copy(selectedFilePath, newFileName, true);

                        Uri fileUri = new Uri(newFileName);
                        Uri baseUri = new Uri(projectDirectory);

                        CurrentCover = newFileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SearchText.IsNullOrEmpty())
                {
                    return;
                }
                SearchAlbums();
            });
        }
        bool DuplicateAlbumName(Album album)
        {
            if (album == null)
            {
                return false;
            }
            var listAlbum = context.Albums.ToList();
            foreach (var item in listAlbum)
            {
                if (item.Title == album.Title)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
