using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibraryWebApplication.Models;

public partial class Review: Entity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    [Display(Name = "Оцінка")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int Rating { get; set; }

    [Display(Name = "Коментар")]
    public string? Comment { get; set; }

    public DateOnly Date { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public string StarsForRating()
    {
        return new string('★', Rating);
    }
}
