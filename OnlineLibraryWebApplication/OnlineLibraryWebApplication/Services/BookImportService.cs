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
        private readonly DblibraryContext context;

        public BookImportService(DblibraryContext context)
        {
            this.context = context;
        }

        private async Task AddBookAsync(IXLRow row, CancellationToken cancellationToken)
        {
            var bookTitle = row.Cell(1).GetValue<string>();
            var publicationYear = row.Cell(3).GetValue<int>(); // Отримання року видання
            var book = await context.Books.FirstOrDefaultAsync(book => book.Title == bookTitle && book.PublicationYear == publicationYear, cancellationToken);
            if (book is null)
            {
                book = new Book { Title = bookTitle, PublicationYear = publicationYear };
                context.Add(book);
            }

            var publisherName = row.Cell(2).GetValue<string>();
            if (!string.IsNullOrEmpty(publisherName))
            {
                var publisher = await context.Publishers.FirstOrDefaultAsync(publisher => publisher.PublisherName == publisherName, cancellationToken);
                if (publisher is null)
                {
                    publisher = new Publisher { PublisherName = publisherName };
                    context.Add(publisher);
                }
                book.Publisher = publisher;
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable", nameof(stream));
            }

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet is null)
            {
                return;
            }

            foreach (var row in worksheet.RowsUsed().Skip(1)) // Пропускаємо заголовок
            {
                await AddBookAsync(row, cancellationToken);
            }
        }
    }
}
