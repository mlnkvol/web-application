using System;
using System.Collections.Generic;

namespace OnlineLibraryWebApplication.Models;

public partial class Category
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<CategoriesBook> CategoriesBooks { get; set; } = new List<CategoriesBook>();
}
