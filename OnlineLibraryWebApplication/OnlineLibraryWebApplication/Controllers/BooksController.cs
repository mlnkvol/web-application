using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Controllers
{
    public class BooksController : Controller
    {
        private readonly DblibraryContext _context;

        public BooksController(DblibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(int? id, string? name, string? filteredBy)
        {
            switch (filteredBy)
            {
                case ("categories"):
                    //знаходження книжок за категорією
                    ViewBag.CategoryId = id;
                    ViewBag.CategoryName = name;
                    ViewBag.PageTitle = "Книги за категорією" + " " + @ViewBag.CategoryName;
                    var booksByCategory = _context.Books
                        .Include(b => b.Publisher)
                        .Include(b => b.Categories)
                        .Include(b => b.Genres)
                        .Include(b => b.Authors)
                        .Where(b => b.Categories
                        .Any(c => c.Id == id));

                    return View(await booksByCategory.ToListAsync());
                case ("publishers"):
                    //знаходження книжок за видавництвом
                    ViewBag.PublisherId = id;
                    ViewBag.PublisherName = name;
                    ViewBag.PageTitle = "Книги видавництва" + " " + @ViewBag.PublisherName;
                    var booksByPublisher = _context.Books
                        .Include(b => b.Publisher)
                        .Include(b => b.Categories)
                        .Include(b => b.Genres)
                        .Include(b => b.Authors)
                        .Where(b => b.PublisherId == id);

                    return View(await booksByPublisher.ToListAsync());
                case ("genres"):
                    //знаходження книжок за жанром
                    ViewBag.GenreId = id;
                    ViewBag.GenreName = name;
                    ViewBag.PageTitle = "Книги за жанром" + " " + @ViewBag.GenreName;
                    var booksByGenres = _context.Books
                        .Where(b => b.Genres.Any(g => g.Id == id))
                        .Include(b => b.Publisher)
                        .Include(b => b.Categories)
                        .Include(b => b.Genres)
                        .Include(b => b.Authors);

                    return View(await booksByGenres.ToListAsync());
                case ("authors"):
                    //знаходження книжок за автором
                    ViewBag.AuthorId = id;
                    ViewBag.Author1 = name;
                    ViewBag.PageTitle = "Книги автора" + " " + @ViewBag.Author1;
                    var booksByAuthors = _context.Books
                        .Include(b => b.Publisher)
                        .Include(b => b.Categories)
                        .Include(b => b.Genres)
                        .Include(b => b.Authors)
                        .Where(b => b.Authors.Any(a => a.Id == id));

                    return View(await booksByAuthors.ToListAsync());
                default:
                    ViewBag.PageTitle = "Усі книги";
                    return View(await _context.Books
                        .Include(b => b.Publisher)
                        .Include(b => b.Categories)
                        .Include(b => b.Genres)
                        .Include(b => b.Authors)
                        .ToListAsync());
            }
        }


        // GET: Books/Details/5
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id");
            return View();
        }

        // POST: Books/Create
        //
        // from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Books/Edit/5
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

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Books/Delete/5
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

        // POST: Books/Delete/5
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
