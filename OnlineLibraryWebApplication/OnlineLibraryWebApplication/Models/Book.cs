﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibraryWebApplication.Models;

public partial class Book: Entity
{
    public int Id { get; set; }

    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Title { get; set; } = null!;

    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Description { get; set; } = null!;

    [Display(Name = "Видавництво")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int PublisherId { get; set; }

    [Display(Name = "Рік публікації")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int PublicationYear { get; set; }

    [Display(Name = "Зображення")]
    public byte[]? Image { get; set; }

    public virtual ICollection<Possession> Possessions { get; set; } = new List<Possession>();

    public virtual Publisher Publisher { get; set; } = null!;

    [NotMapped] 
    public string PublisherName => Publisher?.PublisherName;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
