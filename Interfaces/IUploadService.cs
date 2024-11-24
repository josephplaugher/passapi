using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using passapi.models;
using System.Threading.Tasks;

namespace passapi.Interfaces;

public interface IBaseUploadService
{
    public Task<int> CreateWorksheet(IFormFile file);
    public string GetStringColumnValue(int row, string columnName);
    public Guid GetGuidColumnValue(int row, string columnName);
    public int GetIntColumnValue(int row, string columnName);
}

public interface ITestResultUploadService
{
    Task<JsonResult> Upload(IFormFile file);
    public TEnum GetSubjectColumnValue<TEnum>(int row, string columnName) where TEnum : struct, Enum;
    public Rank GetRankColumnValue(int row, string columnName);
}