using System;
using System.Collections.Generic;

namespace MusicStore.Models;

public partial class ListeningHistory
{
    public int HistoryId { get; set; }

    public int? CustomerId { get; set; }

    public int? PlaylistId { get; set; }

    public DateTime? ListenDate { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Playlist? Playlist { get; set; }
}
