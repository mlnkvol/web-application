using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<GenresBook> GenresBooks { get; set; } = new List<GenresBook>();
}
