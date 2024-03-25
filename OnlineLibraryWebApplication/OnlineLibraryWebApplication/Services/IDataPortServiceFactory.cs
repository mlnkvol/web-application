using OnlineLibraryWebApplication.Models;
using System;

namespace OnlineLibraryWebApplication.Services
{
    public interface IDataPortServiceFactory<TEntityModel>
        where TEntityModel : Entity
    {
        IExportService<TEntityModel> GetExportService(string contentType);
        IImportService<TEntityModel> GetImportService(string contentType);
    }
}
