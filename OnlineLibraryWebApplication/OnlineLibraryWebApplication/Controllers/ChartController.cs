using DocumentFormat.OpenXml.InkML;
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

        [HttpGet("bookTimeline")]
        public async Task<ActionResult<IEnumerable<object>>> GetBookTimelineAsync(CancellationToken cancellationToken)
        {
            var books = await libraryContext.Books.ToListAsync(cancellationToken);
            var timelineData = books.GroupBy(b => b.PublicationYear)
                                    .Select(g => new
                                    {
                                        Year = g.Key,
                                        Books = g.Select(b => new { Title = b.Title, CoverImagePath = $"data:image/jpg;base64,{Convert.ToBase64String(b.Image)}" })
                                    });
            return timelineData.ToList();
        }
    }
}
