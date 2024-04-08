using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLibraryWebApplication.Models;
using OnlineLibraryWebApplication.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLibraryWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IDataPortServiceFactory<Book> _dataPortServiceFactory;
        private readonly DblibraryContext _context; // Додавання контексту бази даних

        public ExportController(IDataPortServiceFactory<Book> dataPortServiceFactory, DblibraryContext context)
        {
            _dataPortServiceFactory = dataPortServiceFactory;
            _context = context; // Ініціалізація контексту бази даних
        }

        [HttpGet("books")]
        public async Task<IActionResult> ExportBooks([FromQuery] string format, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(format))
            {
                return BadRequest("Невірний формат експорту");
            }

            IExportService<Book> exportService;
            if (format.ToLower() == "pdf")
            {
                exportService = new PdfExportService(_context); // Передача контексту у конструктор PdfExportService
            }
            else
            {
                exportService = _dataPortServiceFactory.GetExportService(format);
                if (exportService is null)
                {
                    return BadRequest("Невідомий формат експорту");
                }
            }

            using var stream = new MemoryStream();
            await exportService.WriteToAsync(stream, cancellationToken);
            stream.Position = 0;

            string contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/octet-stream";
            string fileExtension = format.ToLower() == "pdf" ? "pdf" : format;

            return File(stream, contentType, $"books.{fileExtension}");
        }
    }
}
