using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Soar.Interfaces;

public interface IContactUploadService
{
    Task<JsonResult> Upload(IFormFile file, string username);
}

public interface IEventUploadService
{
    Task<JsonResult> Upload(IFormFile file, string username);
}

public interface IBaseUploadService
{
    public Task<int> CreateWorksheet(IFormFile file);
    public string GetStringColumnValue(int row, string columnName);
}
