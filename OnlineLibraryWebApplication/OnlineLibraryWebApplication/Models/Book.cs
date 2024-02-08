using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int AuthorId { get; set; }

    public int CategoryId { get; set; }

    public int GenreId { get; set; }

    public int PublisherId { get; set; }

    public int PublicationYear { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<AuthorsBook> AuthorsBooks { get; set; } = new List<AuthorsBook>();

    public virtual ICollection<CategoriesBook> CategoriesBooks { get; set; } = new List<CategoriesBook>();

    public virtual ICollection<GenresBook> GenresBooks { get; set; } = new List<GenresBook>();

    public virtual ICollection<Possession> Possessions { get; set; } = new List<Possession>();

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
