using System;
using System.Collections.Generic;

namespace MusicStore.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public int? CustomerId { get; set; }

    public string? PlaylistName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsPublic { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
