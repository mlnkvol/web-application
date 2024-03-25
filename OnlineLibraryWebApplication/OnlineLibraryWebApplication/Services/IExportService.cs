using System.IO;
using System.Threading.Tasks;
using OnlineLibraryWebApplication.Models;

namespace OnlineLibraryWebApplication.Services
{
    public interface IExportService<TEntity>
        where TEntity : Entity
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }
}
