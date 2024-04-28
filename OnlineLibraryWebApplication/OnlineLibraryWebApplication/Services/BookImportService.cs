using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Services
{
    public class BookImportService : IImportService<Book>
    {
        private readonly DblibraryContext _context;

        public BookImportService(DblibraryContext context)
        {
            _context = context;
        }

        private async Task AddBookAsync(IXLRow row, CancellationToken cancellationToken)
        {
            var bookTitle = row.Cell(1).GetValue<string>();
            var publicationYear = row.Cell(3).GetValue<int>();

            var publisherName = row.Cell(2).GetValue<string>();
            Publisher publisher = null;
            if (!string.IsNullOrEmpty(publisherName))
            {
                publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.PublisherName == publisherName, cancellationToken);
                if (publisher == null)
                {
                    publisher = new Publisher { PublisherName = publisherName };
                    _context.Publishers.Add(publisher);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == bookTitle && b.PublicationYear == publicationYear, cancellationToken);
            if (book == null)
            {
                book = new Book { Title = bookTitle, PublicationYear = publicationYear, Publisher = publisher };
                _context.Books.Add(book);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable", nameof(stream));
            }

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet != null)
            {
                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    await AddBookAsync(row, cancellationToken);
                }
            }
        }
    }
}
