using System;
using System.Collections.Generic;

namespace MusicStore.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? AlbumId { get; set; }

    public int? CustomerId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Album? Album { get; set; }

    public virtual Customer? Customer { get; set; }
}
