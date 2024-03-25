using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Services
{
    public class BookExportService : IExportService<Book>
    {
        private const string RootWorksheetName = "Books";
        private static readonly IReadOnlyList<string> HeaderNames = new string[]
        {
            "Назва",
            "Видавництво",
            "Рік видання",
            "Автор 1",
            "Автор 2",
        };

        private readonly DblibraryContext context;

        public BookExportService(DblibraryContext context)
        {
            this.context = context;
        }

        private static string GetNameOrDefault(Author? author)
        {
            return author?.Author1 ?? string.Empty;
        }

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private static void WriteBook(IXLWorksheet worksheet, Book book, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = book.Title;
            worksheet.Cell(rowIndex, columnIndex++).Value = book.Publisher?.PublisherName;
            worksheet.Cell(rowIndex, columnIndex++).Value = book.PublicationYear;
            foreach (var author in book.Authors.Take(2))
            {
                worksheet.Cell(rowIndex, columnIndex++).Value = GetNameOrDefault(author);
            }
        }

        private static void WriteBooks(IXLWorksheet worksheet, ICollection<Book> books)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var book in books)
            {
                WriteBook(worksheet, book, rowIndex);
                rowIndex += 1;
            }
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }
            var books = await context.Books
                .Include(book => book.Authors)
                .Include(book => book.Publisher)
                .ToListAsync(cancellationToken);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(RootWorksheetName);
            WriteBooks(worksheet, books);
            workbook.SaveAs(stream);
        }
    }
}
