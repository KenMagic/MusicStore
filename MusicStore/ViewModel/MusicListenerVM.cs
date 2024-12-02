using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStore.Models;
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
using System.Windows.Threading;

namespace MusicStore.ViewModel
{
    public class MusicListenerVM : BaseViewModel
    {
        private ObservableCollection<Playlist> _playlist;
        private Playlist _selectedPlaylist;
        private ObservableCollection<Track> _availableTracks;
        private double _maxProgressValue;
        public double MaxProgressValue
        {
            get => _maxProgressValue;
            set
            {
                _maxProgressValue = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Track> Tracks
        {
            get => _availableTracks;
            set
            {
                _availableTracks = value;
                OnPropertyChanged();
            }
        }
        private Track _selectedTrack;
        public Track CurrentTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();
            }
        }
        private readonly DispatcherTimer _timer;
        private bool _isPaused; 
        private double _progressValue;
        private int _currentIndex;
        private ObservableCollection<string> _playlistTrack;
        private MediaElement _mediaPlayer; 
        public MediaElement MediaPlayer
        { 
            get => _mediaPlayer; 
            set 
            {
                if (_mediaPlayer != value)
                {
                    _mediaPlayer = value;
                    OnPropertyChanged(nameof(MediaPlayer));
                }
            }
        }

        public ObservableCollection<string> Playlist {
            get => _playlistTrack; 
            set {
                _playlistTrack = value; 
                OnPropertyChanged(); 
            } }
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SpeedChangedCommand { get; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand TrackChangedCommand { get; set; }
        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Playlist> Playlists
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged();
            }
        }
        private void InitializePlaylist()
        {
            Playlist = new ObservableCollection<string>();
            foreach (var track in Tracks)
            {       
                Playlist.Add(track.Path);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        { 
            if (MediaPlayer.NaturalDuration.HasTimeSpan) 
            { 
                ProgressValue = MediaPlayer.Position.TotalSeconds; 
            } 
        }
            public MusicListenerVM(MainViewModel mvm)
        {
            MediaPlayer = new MediaElement();
            MediaPlayer.Width = 400;
            MediaPlayer.Height = 60;
            MediaPlayer.MediaOpened += (s, e) =>
            {
                MaxProgressValue = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressValue = 0;
                if (MediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    MessageBox.Show("gud");
                }
            };
            MediaPlayer.MediaFailed += (s, e) =>
            {
                MessageBox.Show("Not gud" + e.ErrorException?.Message);
            };
            MediaPlayer.MediaEnded += (s, e) =>
            {
                NextTrack();
            };
            MediaPlayer.LoadedBehavior = MediaState.Manual;
            MediaPlayer.UnloadedBehavior = MediaState.Stop;
            MediaPlayer.SpeedRatio = 1;
            ProgressValue = 0;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            var context = new MusicStoreContext();
            User current = mvm.User;
            Playlists = new ObservableCollection<Playlist>(context.Playlists
                .Where(p => p.CustomerId == current.CustomerId)
                .Include(p => p.Tracks)
                .ToList());
            PlayCommand = new RelayCommand<object>((p) => { return CurrentTrack != null; }, (p) => { Play(); });
            PauseCommand = new RelayCommand<object>((p) => { return CurrentTrack != null; }, (p) => { Pause(); });
            StopCommand = new RelayCommand<object>((p) => { return CurrentTrack != null; }, (p) => { Stop(); });
            NextCommand = new RelayCommand<object>((p) => { return CurrentTrack != null; }, (p) => { NextTrack(); });
            PreviousCommand = new RelayCommand<object>((p) => { return CurrentTrack != null; }, (p) => { PreviousTrack(); });
            SelectionChangedCommand = new RelayCommand<object>((p) => { return SelectedPlaylist != null && !SelectedPlaylist.Tracks.IsNullOrEmpty(); }, (p) => { UpdatePlaylistDetail(); });
            TrackChangedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (CurrentTrack != null)
                {
                    _currentIndex = Tracks.IndexOf(CurrentTrack);
                    MediaPlayer.Source = new Uri(CurrentTrack.Path, UriKind.Relative);
                }
            });
        }
        private void UpdatePlaylistDetail()
        {
            Tracks = new ObservableCollection<Track>(SelectedPlaylist.Tracks);
            InitializePlaylist();
            CurrentTrack = Tracks.First();
        }
        private void Play() 
        { 
            if (_isPaused) 
            { 
                MediaPlayer.Play();
                _isPaused = false;
            } 
            else 
            { 
                PlayCurrentSong(); 
            } 
            _timer.Start(); 
        }
        private void Pause() 
        { 
            MediaPlayer.Pause();
            _isPaused = true;
            _timer.Stop(); 
        }
        private void Stop() 
        { 
            MediaPlayer.Stop();
            _timer.Stop();
            ProgressValue = 0;
            _isPaused = false; 
        }
        private void NextTrack() { PlayNextSong(); }
        private void PreviousTrack() 
        { 
            if (_currentIndex > 0) 
            { 
                _currentIndex--; 
            } 
            else 
            { 
                _currentIndex = Playlist.Count - 1; 
            } 
            PlayCurrentSong(); 
        }
        private void PlayCurrentSong()
        {
            if (Playlist.Count > 0)
            {
                MediaPlayer.Source = new Uri(Playlist[_currentIndex], UriKind.Relative);
                CurrentTrack = Tracks.FirstOrDefault(t => t.Path == Playlist[_currentIndex]);
                MediaPlayer.Play();
                ProgressValue = 0;
                _isPaused = false;
            }
        }
        private void PlayNextSong() 
        {
            if (_currentIndex < Playlist.Count - 1)
            { 
                _currentIndex++; 
            } 
            else if (_currentIndex >= Playlist.Count - 1) 
            {
                _currentIndex = 0; 
            } 
            PlayCurrentSong(); 
        }
        private void ChangeSpeed(double speed) { MediaPlayer.SpeedRatio = speed; }
    }
}
