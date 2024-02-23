using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibraryWebApplication.Models;

public partial class Genre
{
    public int Id { get; set; }

    [Display(Name = "Жанр")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string GenreName { get; set; } = null!;

    [NotMapped]
    public int BookCount { get; set; }
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
