using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DblibraryContext _context;
        public ChartController(DblibraryContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var categories = _context.Categories.ToList();
            List<object> catBook = new List<object>();
            catBook.Add(new object[] { "Категорія", "Кількість книжок" });

            foreach (var c  in categories)
            {
                var bookCount = _context.Books.Count(b => b.Categories.Any(bookCategory => bookCategory.Id == c.Id));
                catBook.Add(new object[] { c.CategoryName, bookCount });
            }

            return new JsonResult(catBook);

        }

        [HttpGet("Genres")]
        public JsonResult Genres(int categoryId)
        {
            var genres = _context.Genres
                .Where(g => g.Books.Any(b => b.Categories.Any(c => c.Id == categoryId)))
                .ToList();

            List<object> genBook = new List<object>();
            genBook.Add(new object[] { "Жанр", "Кількість книжок" });

            foreach (var g in genres)
            {
                var bookCount = _context.Books.Count(b => b.Genres.Any(bookGenre => bookGenre.Id == g.Id));
                genBook.Add(new object[] { g.GenreName, bookCount });
            }

            return new JsonResult(genBook);
        }


    }
}
