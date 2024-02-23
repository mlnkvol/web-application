using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibraryWebApplication.Models;

public partial class Publisher
{
    public int Id { get; set; }

    [Display(Name = "Видавництво")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string PublisherName { get; set; } = null!;

    [NotMapped]
    public int BookCount { get; set; }
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
