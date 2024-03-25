using System.IO;
using System.Threading.Tasks;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Services
{
    public interface IImportService<TEntity>
        where TEntity : Entity
    {
        Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}

