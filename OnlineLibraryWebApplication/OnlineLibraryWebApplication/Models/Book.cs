using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int PublisherId { get; set; }

    public int PublicationYear { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<Possession> Possessions { get; set; } = new List<Possession>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
