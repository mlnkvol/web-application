using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class Author
{
    public int Id { get; set; }

    public string Author1 { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
