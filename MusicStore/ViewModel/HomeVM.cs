using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MusicStore.Models;
namespace MusicStore.ViewModel
{
    public class HomeVM : BaseViewModel
    {
        private List<Artist> _listArtist;
        private MainViewModel _mainViewModel;
        private List<Album> _listAlbum;
        public List<Artist> listArtist
        {
            get => _listArtist;
            set
            {
                _listArtist = value;
                OnPropertyChanged();
            }
        }
        public List<Album> ListAlbums
        {
            get => _listAlbum;
            set
            {
                _listAlbum = value;
                OnPropertyChanged();
            }
        }
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterItems();
            }
        }
        public ICommand ArtistView { get; set; }

        public ICommand AlbumView { get; set; }
        public HomeVM(MainViewModel mvm)
        {
            _mainViewModel = mvm;
            var context = new MusicStoreContext();
            SearchQuery = mvm.SearchQuery;
            FilterItems();
            ArtistView = new RelayCommand<TextBlock>((p) => { return true; }, (p) =>
            {
                var context = new MusicStoreContext();
                int id = int.Parse(p.Text);
                var artist = context.Artists.FirstOrDefault(a => a.ArtistId == id);
                _mainViewModel.ChangePage(new ArtistPage(mvm, artist));
            });
            AlbumView = new RelayCommand<TextBlock>((p) => { return true; }, (p) =>
            {
                var context = new MusicStoreContext();
                int id = int.Parse(p.Text);
                var album = context.Albums.FirstOrDefault(a => a.AlbumId == id);
                _mainViewModel.ChangePage(new AlbumPage(mvm, album));
            });
        }
        private void FilterItems()
        {
            var context = new MusicStoreContext();

            // Filter artists and albums based on the search query
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                listArtist = context.Artists
    .AsEnumerable()
    .Where(a => a.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
    .ToList();

                ListAlbums = context.Albums
                    .AsEnumerable()
                    .Where(a => a.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                // If there's no search query, show all items
                listArtist = context.Artists.ToList();
                ListAlbums = context.Albums.ToList();
            }
        }
    }
}
