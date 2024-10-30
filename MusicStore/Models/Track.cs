using System;
using System.Collections.Generic;

namespace MusicStore.Models;

public partial class Track
{
    public int TrackId { get; set; }

    public int? AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public TimeOnly? Duration { get; set; }

    public int? TrackNumber { get; set; }

    public string? FilePath { get; set; }

    public string? CoverImage { get; set; }

    public virtual Album? Album { get; set; }
}
