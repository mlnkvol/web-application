using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Services
{
    public class BookDataPortServiceFactory : IDataPortServiceFactory<Book>
    {
        private readonly DblibraryContext _libraryContext;

        public BookDataPortServiceFactory(DblibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public IExportService<Book> GetExportService(string contentType)
        {
            return new BookExportService(_libraryContext);
        }

        public IImportService<Book> GetImportService(string contentType)
        {
            return new BookImportService(_libraryContext);
        }
    }
}
