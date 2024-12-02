using Microsoft.IdentityModel.Tokens;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicStore.ViewModel
{
    public class ArtistMVM : BaseViewModel
    {
        private string _searchText;
        private List<Artist> _artists;
        private Artist _selectedArtist;
        private string _artistName;
        private string _artistGenre;
        private string _artistBiography;
        private MusicStoreContext context = new MusicStoreContext();
        public ObservableCollection<string> AvailableGenres { get; set; }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchArtists();
            }
        }



        public List<Artist> Artists
        {
            get => _artists;
            set
            {
                _artists = value;
                OnPropertyChanged();
            }
        }
        public Artist SelectedArtist
        {
            get => _selectedArtist;
            set
            {
                _selectedArtist = value;
                OnPropertyChanged();
                if (value != null)
                {
                    CurrentName = value.Name;
                    CurrentGenre = value.Genre;
                    CurrentBiography = value.Biography;
                }
            }
        }
        public string CurrentName
        {
            get => _artistName;
            set
            {
                _artistName = value;
                OnPropertyChanged();
            }
        }
        public string CurrentGenre
        {
            get => _artistGenre;
            set
            {
                _artistGenre = value;
                OnPropertyChanged();
            }
        }
        public string CurrentBiography
        {
            get => _artistBiography;
            set
            {
                _artistBiography = value;
                OnPropertyChanged();
            }
        }
        public ICommand AddArtistCommand { get; set; }
        public ICommand UpdateArtistCommand { get; set; }
        public ICommand DeleteArtistCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public ArtistMVM()
        {
            AvailableGenres = new ObservableCollection<string>
        {
            "Rock",
            "Pop",
            "Jazz",
            "Classical",
            "Hip-Hop",
            "Country"
        };
            Artists = context.Artists.ToList();
            AddArtistCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                var context = new MusicStoreContext();
                var artist = new Artist()
                {
                    Name = CurrentName,
                    Genre = CurrentGenre,
                    Biography = CurrentBiography
                };
                context.Artists.Add(artist);
                context.SaveChanges();
                Artists = context.Artists.ToList();
            });
            UpdateArtistCommand = new RelayCommand<object>((p) => { return SelectedArtist != null; }, (p) =>
            {
                if (!CurrentName.IsNullOrEmpty() && !CurrentGenre.IsNullOrEmpty())
                {
                    var context = new MusicStoreContext();
                    var artist = context.Artists.Find(SelectedArtist.ArtistId);
                    artist.Name = CurrentName;
                    artist.Genre = CurrentGenre;
                    artist.Biography = CurrentBiography;
                    context.SaveChanges();
                    Artists = context.Artists.ToList();
                }
            });
            DeleteArtistCommand = new RelayCommand<object>((p) => { return SelectedArtist != null; }, (p) =>
            {
                var context = new MusicStoreContext();
                var artist = context.Artists.Find(SelectedArtist.ArtistId);
                context.Artists.Remove(artist);
                context.SaveChanges();
                Artists = context.Artists.ToList();
            });
            RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                CurrentName = "";
                CurrentGenre = "";
                CurrentBiography = "";
                Artists = new MusicStoreContext().Artists.ToList();
            });
            SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (SearchText.IsNullOrEmpty())
                {
                    return;
                }
                SearchArtists();
            });
        }
        private void SearchArtists()
        {
            Artists = context.Artists.Where(a => a.Name.ToLower().Contains(SearchText.ToLower())).ToList();
        }
    }
}
