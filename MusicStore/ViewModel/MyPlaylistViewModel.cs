using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MusicStore.Models;
using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;


namespace MusicStore.ViewModel
{
    public class MyPlaylistViewModel : BaseViewModel
    {
        private MusicStoreContext _context; // Use the context for DB access
        private ObservableCollection<Playlist> _playlists;
        private ObservableCollection<Track> _availableTracks;
        private Playlist _selectedPlaylist;
        private Track _selectedTrack;
        private string _playlistName;
        private bool _isPublic;
        public string PlaylistName
        {
            get => _playlistName;
            set
            {
                _playlistName = value;
                OnPropertyChanged();
            }
        }
        public bool IsPublic
        {
            get => _isPublic;
            set
            {
                _isPublic = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        public ObservableCollection<Track> SelectedPlaylistTracks
        {
            get => _availableTracks;
            set
            {
                _availableTracks = value;
                OnPropertyChanged();
            }
        }

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged(nameof(SelectedTrack));
            }
        }


        // Commands
        public ICommand AddPlaylistCommand { get; set; }
        public ICommand DeletePlaylistCommand { get; set; }
        public ICommand UpdatePlaylistCommand { get; set; }
        public ICommand AddTrackToPlaylistCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand RemoveTrackFromPlaylistCommand { get; set; }
        public MyPlaylistViewModel(MainViewModel mvm)
        {
            _context = new MusicStoreContext();  // Inject MusicStoreContext
            Playlists = new ObservableCollection<Playlist>(_context.Playlists
    .Where(p => p.CustomerId == mvm.User.CustomerId)
    .Include(p => p.Tracks)// Filter by current user's ID
    .ToList());
            AddPlaylistCommand = new RelayCommand<object>((p) => !ValidateSelectedPlayList(p), (p) => { AddPlaylist(p, mvm); });
            DeletePlaylistCommand = new RelayCommand<object>((p) => ValidateSelectedPlayList(p), (p) => { DeletePlaylist(p, mvm); });
            RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Refresh(mvm); });
            UpdatePlaylistCommand = new RelayCommand<object>((p) => ValidateSelectedPlayList(p), (p) => { UpdatePlaylist(p, mvm); });
            SelectionChangedCommand = new RelayCommand<object>((p) => ValidateSelectedPlayList(p), (p) => { UpdatePlaylistDetail(); });
            RemoveTrackFromPlaylistCommand = new RelayCommand<object>((p) => ValidateSelectedTrack(p), (p) => { RemoveTrack(); });

        }

        private void RemoveTrack()
        {
            if (SelectedPlaylistTracks != null && SelectedPlaylist != null)
            {
                SelectedPlaylist.Tracks.Remove(SelectedTrack);
                _context.SaveChanges();
            }
            UpdatePlaylistDetail();

        }

        private void UpdatePlaylistDetail()
        {
            PlaylistName = SelectedPlaylist.PlaylistName;
            IsPublic = (bool)SelectedPlaylist.IsPublic;
            SelectedPlaylistTracks = new ObservableCollection<Track>(SelectedPlaylist.Tracks);
        }

        private void DeletePlaylist(object p, MainViewModel mvm)
        {
            var current = _context.Playlists.Find(SelectedPlaylist.PlaylistId);
            if (current != null)
            {
                foreach (var playlistTrack in current.Tracks.ToList())
                {
                    current.Tracks.Remove(playlistTrack);
                }
            }
            _context.SaveChanges();

            _context.Playlists.Remove(SelectedPlaylist);
            _context.SaveChanges();

            // Update the view model to reflect the deletion
            Refresh(mvm);
        }

        private void AddPlaylist(object p, MainViewModel mvm)
        {
            if (string.IsNullOrEmpty(PlaylistName))
            {
                return;
            }
            var membership = _context.Memberships
                .Where(m => m.CustomerId == mvm.User.CustomerId)
                .Where(m => m.Status == "Active")
                .FirstOrDefault();
            if(membership == null)
            {
                MessageBox.Show("You need to have an active membership to create a playlist");
                return;
            }
            var current = new Playlist();
            current.PlaylistName = PlaylistName;
            current.CustomerId = mvm.User.CustomerId;
            current.CreatedAt = DateTime.Now;
            current.IsPublic = IsPublic;

            _context.Playlists.Add(current);
            _context.SaveChanges();
            Refresh(mvm);
        }
        private void UpdatePlaylist(object p, MainViewModel mvm)
        {

            var playlistToUpdate = _context.Playlists
                                    .FirstOrDefault(p => p.PlaylistId == SelectedPlaylist.PlaylistId);

            if (playlistToUpdate == null)
            {
                return;
            }

            playlistToUpdate.PlaylistName = PlaylistName;  // Update playlist name
            playlistToUpdate.IsPublic = IsPublic;  // Update public/private status

            _context.SaveChanges();  // Commit the changes to the database

            // Refresh the playlist collection to reflect the updated data
            Refresh(mvm);
        }

        bool ValidateSelectedPlayList(object p)
        {
            return SelectedPlaylist != null;
        }
        bool ValidateSelectedTrack(object p)

        {
            return SelectedTrack != null;
        }


        private void Refresh(MainViewModel mvm)
        {
            Playlists = new ObservableCollection<Playlist>(_context.Playlists
    .Where(p => p.CustomerId == mvm.User.CustomerId)  // Filter by current user's ID
    .ToList());
            SelectedPlaylist = null;
            SelectedTrack = null;
            PlaylistName = "";
            IsPublic = false;
            SelectedPlaylistTracks = null;
        }

        // CRUD operations for Playlist

    }
}
