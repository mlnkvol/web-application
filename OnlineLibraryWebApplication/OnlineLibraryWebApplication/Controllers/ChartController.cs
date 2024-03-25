using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private record CountByCategoryResponseItem(string Category, int Count);
        private record CountByGenreResponseItem(string Genre, int Count);
        private readonly DblibraryContext libraryContext;
        public ChartsController(DblibraryContext libraryContext)
        {
            this.libraryContext = libraryContext;
        }

        [HttpGet("countByCategories")]
        public async Task<JsonResult> GetCountByCategoriesAsync(CancellationToken cancellationToken)
        {
            var responseItems = await libraryContext.Categories
                .Select(category => new CountByCategoryResponseItem(category.CategoryName, category.Books.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }

        [HttpGet("countByGenres")]
        public async Task<JsonResult> GetCountByGenresAsync(CancellationToken cancellationToken)
        {
            var responseItems = await libraryContext.Genres
                .OrderByDescending(genre => genre.Books.Count())
                .Take(5)
                .Select(genre => new CountByGenreResponseItem(genre.GenreName, genre.Books.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }

        [HttpGet("countByGenres2")]
        public async Task<JsonResult> GetCountByGenres2Async(CancellationToken cancellationToken)
        {
            var responseItems = await libraryContext.Genres
                .OrderBy(genre => genre.Books.Count())
                .Take(5)
                .Select(genre => new CountByGenreResponseItem(genre.GenreName, genre.Books.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }
    }
}
