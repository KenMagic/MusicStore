using Microsoft.EntityFrameworkCore;
using MusicStore.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MusicStore.ViewModel
{
    public class PlaylistViewModel : BaseViewModel
    {
        private string _searchText;
        private Playlist _selectedPlaylist;
        private ObservableCollection<Playlist> _filteredPlaylists;
        private MusicStoreContext context = new MusicStoreContext();
        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<Playlist> FilteredPlaylists
        {
            get => _filteredPlaylists;
            set
            {
                _filteredPlaylists = value;
                OnPropertyChanged();
            }
        }

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
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
                FilterPlaylists();
            }
        }

        public ICommand AddToPlaylistCommand { get; }

        public PlaylistViewModel(object c, MainViewModel mvm)
        {
            Playlists = new ObservableCollection<Playlist>(context.Playlists
    .Where(p => p.CustomerId == mvm.User.CustomerId)  // Filter by current user's ID
    .ToList());
            FilteredPlaylists = new ObservableCollection<Playlist>(Playlists);
            AddToPlaylistCommand = new RelayCommand<Playlist>(CanExecuteAddToPlaylist, (p) => { AddToPlaylist(p, c); });
        }

        private void AddToPlaylist(Playlist playlist, object c)
        {
            if (c is Track t)
            {
                var plToUpdate = context.Playlists.Include(p => p.Tracks).FirstOrDefault(p => p.PlaylistId == playlist.PlaylistId);
                if (plToUpdate != null)
                {
                    var trackToAdd = context.Tracks.FirstOrDefault(track => track.TrackId == t.TrackId);
                    if (trackToAdd != null && !plToUpdate.Tracks.Any(existingTrack => existingTrack.TrackId == trackToAdd.TrackId))
                    {
                        plToUpdate.Tracks.Add(trackToAdd);
                        context.SaveChanges();
                        MessageBox.Show("Đã thêm");
                    }
                    else
                    {
                        MessageBox.Show("Playlist đã có bài nhạc này");
                    }
                }
            }
            else if (c is Album a)
            {
                var plToUpdate = context.Playlists.Include(p => p.Tracks).FirstOrDefault(p => p.PlaylistId == playlist.PlaylistId);
                if (plToUpdate != null) 
                {
                    var albumTracks = context.Tracks.Where(t => t.AlbumId == a.AlbumId).ToList(); 
                    foreach (var track in albumTracks)
                    {
                        var trackToAdd = context.Tracks.FirstOrDefault(tr => tr.TrackId == track.TrackId);
                        if (trackToAdd != null && !plToUpdate.Tracks.Any(existingTrack => existingTrack.TrackId == trackToAdd.TrackId)) 
                        {
                            plToUpdate.Tracks.Add(trackToAdd); } 
                    }
                    context.SaveChanges(); MessageBox.Show("Đã thêm");
                }
            }
        }

        private bool CanExecuteAddToPlaylist(Playlist playlist)
        {
            return playlist != null;
        }

        private void FilterPlaylists()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredPlaylists = new ObservableCollection<Playlist>(Playlists);
            }
            else
            {
                FilteredPlaylists = new ObservableCollection<Playlist>(Playlists
                    .Where(p => p.PlaylistName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            }
        }

    }
}
