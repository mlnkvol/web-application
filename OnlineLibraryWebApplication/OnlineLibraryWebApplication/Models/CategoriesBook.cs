using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class CategoriesBook
{
    public int CategoryId { get; set; }

    public int BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}
