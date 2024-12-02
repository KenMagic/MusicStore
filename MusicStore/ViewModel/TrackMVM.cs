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
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStore.ViewModel
{
    public class TrackMVM : BaseViewModel
    {
        private string _searchText;
        private List<Track> _tracks;
        private Track _selectedTrack;
        private string _trackTitle;
        private Album _trackAlbum;
        private string _trackFile;
        private MusicStoreContext context = new MusicStoreContext();
        private TimeOnly _duration; 
        public TimeOnly Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchTracks();
            }
        }
        public List<Track> Tracks
        {
            get => _tracks;
            set
            {
                _tracks = value;
                OnPropertyChanged();
            }
        }
        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();
                if (value != null)
                {
                    CurrentTitle = value.Title;
                    SelectedAlbum = value.Album;
                    CurrentFile = value.Path;
                }
            }
        }
        public string CurrentTitle
        {
            get => _trackTitle;
            set
            {
                _trackTitle = value;
                OnPropertyChanged();
            }
        }
        public Album SelectedAlbum
        {
            get => _trackAlbum;
            set
            {
                _trackAlbum = value;
                OnPropertyChanged();
            }
        }
        public string CurrentFile
        {
            get => _trackFile;
            set
            {
                _trackFile = value;
                OnPropertyChanged();
            }
        }
        public void SearchTracks()
        {
            Tracks = context.Tracks.Where(t => t.Title.Contains(SearchText)).ToList();
        }
        public ICommand AddTrackCommand { get; set; }
        public ICommand DeleteTrackCommand { get; set; }
        public ICommand UpdateTrackCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand BrowseCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public TrackMVM()
        {
            Tracks = context.Tracks.ToList();
            AddTrackCommand = new RelayCommand<object>((p) => { return SelectedTrack==null; }, (p) => { AddTrack(); });
            UpdateTrackCommand = new RelayCommand<object>((p) => { return SelectedTrack != null; }, (p) => { UpdateTrack(); });
            DeleteTrackCommand = new RelayCommand<object>((p) => { return SelectedTrack!=null; }, (p) => { DeleteTrack(); });
            RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Refresh(); });
            BrowseCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Browse(); });
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SearchText.IsNullOrEmpty())
                {
                    return;
                }
                SearchTracks();
            });
        }
        private void AddTrack()
        {
            if (string.IsNullOrEmpty(CurrentTitle) || SelectedAlbum == null || string.IsNullOrEmpty(CurrentFile))
            {
                return;
            }
            var track = new Track()
            {
                Title = CurrentTitle,
                AlbumId = SelectedAlbum.AlbumId,
                Path = CurrentFile,
                Duration = Duration
            };
            context.Tracks.Add(track);
            context.SaveChanges();
            Tracks = context.Tracks.ToList();
        }
        private void UpdateTrack()
        {
            if (string.IsNullOrEmpty(CurrentTitle) || SelectedAlbum == null || string.IsNullOrEmpty(CurrentFile))
            {
                return;
            }
            var track = context.Tracks.Find(SelectedTrack.TrackId);
            track.Title = CurrentTitle;
            track.AlbumId = SelectedAlbum.AlbumId;
            track.Path = CurrentFile;
            track.Duration = Duration;
            context.SaveChanges();
            Tracks = context.Tracks.ToList();
        }
        private void DeleteTrack()
        {
            var track = context.Tracks.Find(SelectedTrack.TrackId);
            context.Tracks.Remove(track);
            context.SaveChanges();
            Tracks = context.Tracks.ToList();
        }
        private void Refresh()
        {
            CurrentTitle = "";
            SelectedAlbum = null;
            CurrentFile = "";
            SelectedTrack = null;
            Tracks = context.Tracks.ToList();
        }
        private void Browse()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Audio Files|*.mp3;*.wav;*.flac";

            if (dialog.ShowDialog() == true)
            {
                string selectedFilePath = dialog.FileName;

                string audioDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio");

                if (!Directory.Exists(audioDirectory))
                {
                    Directory.CreateDirectory(audioDirectory);
                }

                string fileName = Path.GetFileName(selectedFilePath);

                string destinationPath = Path.Combine(audioDirectory, fileName);

                try
                {
                    File.Copy(selectedFilePath, destinationPath, overwrite: true);
                    CurrentFile = destinationPath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
