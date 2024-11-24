using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Collections.Generic;
using passapi.Interfaces;
using passapi.data;
using passapi.models;

namespace passapi.Services;

public class BaseUploadService : IBaseUploadService
{
    protected readonly AppDbContext _context;
    protected DataUploadResponse response = new();
    protected readonly Dictionary<string, int> excelHeaders = [];
    protected int rowCount = 0;
    protected ExcelPackage package;
    protected ExcelWorksheet worksheet;
    protected int columnCount = 0;

    public BaseUploadService(AppDbContext context)
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

        if (worksheet == null)
        {
            throw new Exception("failed to create worksheet");
        }
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

    public Guid GetGuidColumnValue(int row, string columnName)
    {
        excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber);
        return (Guid)worksheet.Cells[row, columnNumber].Value;
    }

    public double GetDoubleColumnValue(int row, string columnName)
    {
        excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber);
        return (double)worksheet.Cells[row, columnNumber].Value;
    }

    // public float GetFloatColumnValue(int row, string columnName)
    // {
    //     excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber);
    //     return (float)worksheet.Cells[row, columnNumber].Value;
    // }

    public int GetIntColumnValue(int row, string columnName)
    {
        excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber);

        // Get the value from the cell
        object cellValue = worksheet.Cells[row, columnNumber].Value;

        // Try to convert it to an integer
        if (cellValue is double doubleValue)
        {
            return (int)doubleValue; // Explicit cast from double to int
        }
        else if (cellValue is int intValue)
        {
            return intValue; // Already an int
        }
        else if (int.TryParse(cellValue?.ToString(), out int parsedInt))
        {
            return parsedInt; // Parse from string if needed
        }

        throw new InvalidCastException($"Unable to convert value in row {row}, column {columnName} to an int.");
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
