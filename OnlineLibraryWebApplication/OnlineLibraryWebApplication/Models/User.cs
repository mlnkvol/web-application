using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class User: Entity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public virtual ICollection<Possession> Possessions { get; set; } = new List<Possession>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
