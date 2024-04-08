using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Services
{
    public class PdfExportService : IExportService<Book>
    {
        private readonly DblibraryContext _context;

        public PdfExportService(DblibraryContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            var books = await _context.Books
                .Include(book => book.Authors)
                .Include(book => book.Publisher)
                .ToListAsync(cancellationToken);

            var document = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10, Bottom = 10, Left = 10, Right = 10 }
                }
            };

            foreach (var book in books)
            {
                var htmlContent = $@"
                    <html>
                    <head>
                        <title>{book.Title}</title>
                    </head>
                    <body>
                        <h1>{book.Title}</h1>
                        <p>Видавництво: {book.Publisher?.PublisherName}</p>
                        <p>Рік видання: {book.PublicationYear}</p>
                        <p>Автори: {string.Join(", ", book.Authors.Select(author => author.Author1))}</p>
                    </body>
                    </html>
                ";

                document.Objects.Add(new ObjectSettings
                {
                    HtmlContent = htmlContent
                });
            }

            var converter = new SynchronizedConverter(new PdfTools());
            var pdfBytes = converter.Convert(document);

            await stream.WriteAsync(pdfBytes, 0, pdfBytes.Length, cancellationToken);
        }
    }
}
