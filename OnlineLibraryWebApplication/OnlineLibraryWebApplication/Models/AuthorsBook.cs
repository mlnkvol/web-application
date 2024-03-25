﻿using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class AuthorsBook: Entity
{
    public int AuthorId { get; set; }

    public int BookId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
