using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Data.Identity;
using OnlineLibraryWebApplication.Models;
using OnlineLibraryWebApplication.Services;
using System.Security.Cryptography;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineLibraryWebApplication.Controllers
{
    public class BooksController : Controller
    {
        private readonly DblibraryContext _context;
        private readonly IDataPortServiceFactory<Book> _bookDataPortServiceFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(DblibraryContext context, IDataPortServiceFactory<Book> bookDataPortServiceFactory, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _bookDataPortServiceFactory = bookDataPortServiceFactory;
            _userManager = userManager;
        }

        //// POST: Possessions/AddToLibrary/5
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> AddToLibrary(int id)
        //{
        //    // Отримуємо UserId як string
        //    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        //        ?? throw new UnauthorizedAccessException("Користувач не авторизований");

        //    // Перевіряємо, чи книга вже є в бібліотеці користувача
        //    var possession = await _context.Possessions
        //        .FirstOrDefaultAsync(p => p.UserId == userId && p.BookId == id);
        //    if (possession != null)
        //    {
        //        return BadRequest("Ця книга вже є у вашій бібліотеці.");
        //    }

        //    // Додаємо нову книгу до бібліотеки
        //    _context.Possessions.Add(new Possession
        //    {
        //        UserId = userId,
        //        BookId = id,
        //        StartTime = DateTime.Now,
        //        Accessibility = 1,
        //        EndTime = DateTime.Now.AddYears(1),
        //        CurrentPage = 0
        //    });

        //    await _context.SaveChangesAsync();
        //    return Ok("Книга успішно додана до вашої бібліотеки.");
        //}

        // Решта методів залишається без змін
        public async Task<IActionResult> Index(int? id, string? name, string? filteredBy, string? search)
        {
            IQueryable<Book> books = _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .Include(b => b.Genres)
                .Include(b => b.Authors);

            switch (filteredBy)
            {
                case "categories":
                    ViewBag.CategoryId = id;
                    ViewBag.CategoryName = name;
                    ViewBag.PageTitle = "Книги за категорією " + ViewBag.CategoryName;
                    books = books.Where(b => b.Categories.Any(c => c.Id == id));
                    break;

                case "publishers":
                    ViewBag.PublisherId = id;
                    ViewBag.PublisherName = name;
                    ViewBag.PageTitle = "Книги видавництва " + ViewBag.PublisherName;
                    books = books.Where(b => b.PublisherId == id);
                    break;

                case "genres":
                    ViewBag.GenreId = id;
                    ViewBag.GenreName = name;
                    ViewBag.PageTitle = "Книги за жанром " + ViewBag.GenreName;
                    books = books.Where(b => b.Genres.Any(g => g.Id == id));
                    break;

                case "authors":
                    ViewBag.AuthorId = id;
                    ViewBag.Author1 = name;
                    ViewBag.PageTitle = "Книги автора " + ViewBag.Author1;
                    books = books.Where(b => b.Authors.Any(a => a.Id == id));
                    break;

                default:
                    ViewBag.PageTitle = "Всі книги";
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search));
                ViewBag.PageTitle = "Результати пошуку за: " + search;
            }

            return View(await books.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Genres)
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var genreIds = book.Genres?.Select(g => g.Id).ToList() ?? new List<int>();
            var recommendedBooks = new List<Book>();

            if (genreIds.Any())
            {
                recommendedBooks = await _context.Books
                    .Include(b => b.Genres)
                    .Where(b => b.Id != id && b.Genres.Any(g => genreIds.Contains(g.Id)))
                    .Take(5)
                    .ToListAsync();
            }

            ViewBag.RecommendedBooks = recommendedBooks;

            return View(book);
        }

        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,PublisherId,PublicationYear,Image")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", book.PublisherId);
            return View(book);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile booksFile, CancellationToken cancellationToken)
        {
            try
            {
                if (booksFile == null || booksFile.Length == 0)
                {
                    ModelState.AddModelError("booksFile", "Please select a file to upload.");
                    return View();
                }

                var importService = this._bookDataPortServiceFactory.GetImportService(booksFile.ContentType);
                using var stream = booksFile.OpenReadStream();
                await importService.ImportFromStreamAsync(stream, cancellationToken);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while importing the file. Please try again.");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", CancellationToken cancellationToken = default)
        {
            var exportService = _bookDataPortServiceFactory.GetExportService(contentType);
            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"books_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", book.PublisherId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,PublisherId,PublicationYear,Image")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", book.PublisherId);
            return View(book);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}