using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.ViewModel
{
    public class AppManageVM : BaseViewModel
    {
        public ArtistMVM ArtistMVM { get; set; }
        public AlbumMVM AlbumMVM { get; set; }
        public GenreMVM GenreMVM { get; set; }
        public TrackMVM TrackMVM { get; set; }
        public AppManageVM()
        {
            ArtistMVM = new ArtistMVM();
            AlbumMVM = new AlbumMVM();
            GenreMVM = new GenreMVM();
            TrackMVM = new TrackMVM();
        }
    }
}
