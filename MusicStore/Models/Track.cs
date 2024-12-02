using System;
using System.Collections.Generic;

namespace MusicStore.Models;

public partial class Track
{
    public int TrackId { get; set; }

    public int? AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public TimeOnly? Duration { get; set; }

    public string? Path { get; set; }

    public string? CoverImage { get; set; }

    public virtual Album? Album { get; set; }

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
