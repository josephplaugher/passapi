using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Collections.Generic;

namespace Soar.Services;

public class BaseUploadService : IBaseUploadService
{
    protected readonly SoarContext _context;
    protected DataUploadResponse response = new();
    protected readonly Dictionary<string, int> excelHeaders = [];
    protected int rowCount = 0;
    protected ExcelPackage package;
    protected ExcelWorksheet worksheet;
    protected int columnCount = 0;
    protected Contact contact;

    public BaseUploadService(SoarContext context)
    {
        _context = context;
    }


    public async Task<int> CreateWorksheet(IFormFile file)
    {
        using MemoryStream stream = new();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        package = new(stream);
        worksheet = package.Workbook.Worksheets[0];

        rowCount = worksheet.Dimension.Rows;
        columnCount = worksheet.Dimension.Columns;
        response.TotalRowsProcessed = rowCount - 1; //minus 1 accounts for header row

        for (int col = 1; col <= columnCount; col++)
        {
            string columnName = worksheet.Cells[1, col].Value?.ToString().Trim();
            excelHeaders.Add(columnName.Replace(" ", "").ToLower(), col);
        }

        return 0;
    }

    public string GetStringColumnValue(int row, string columnName)
    {
        return excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber) ? worksheet.Cells[row, columnNumber].Value?.ToString() : "";
    }

    public DateTime GetDateColumnValue(int row, string columnName)
    {
        if (!excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber))
        {
            return DateTime.MinValue; // Return empty date if the columnName is not found
        }

        if (DateTime.TryParse(worksheet.Cells[row, columnNumber].Value?.ToString(), out DateTime dateValue))
        {
            return dateValue; // Return short date format if the value is a valid date
        }
        else
        {
            throw new Exception($"Invalid date format in row {row} and column {columnName}");
        }
    }
}
