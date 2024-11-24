using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passapi.data;
using passapi.Interfaces;
using passapi.models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace passapi.Services;

public class TestResultUploadService : BaseUploadService, ITestResultUploadService
{
    // private readonly AppDbContext context;
    private List<TestResult> testResults = new List<TestResult>();

    public TestResultUploadService(AppDbContext context) : base(context)
    {
    }

    public async Task<JsonResult> Upload(IFormFile file)
    {
        try
        {
            excelHeaders.Clear();
            if (file == null || file.Length == 0)
            {
                throw new Exception("Upload a file.");
            }
            await CreateWorksheet(file);

            // loop through each row in the excel file and do the stuff
            for (int row = 2; row < rowCount; row++)
            {
                bool rowNotEmpty = CheckForResultOnRow(row);
                if (rowNotEmpty == true)
                {
                    TestResult thisResult = AddResult(row);
                    
                    testResults.Add(thisResult);
                }
            }

            _context.TestResults.AddRange(testResults);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            response.Errors.Add(ex.Message);
        }

        return new JsonResult(response);
    }

    public TEnum GetSubjectColumnValue<TEnum>(int row, string columnName) where TEnum : struct, Enum
    {
        // excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber);
        // var cell = worksheet.Cells[row, columnNumber];
        // string subject = cell?.Value.ToString() ?? string.Empty;
        // Console.WriteLine("", subject);
        // // Subject subject = (Subject)Enum.Parse(typeof(Subject), valueFromColumn, true);
        // return Subject.L;
        string t = "";
        if (excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber))
        {
            var cellValue = worksheet.Cells[row, columnNumber].Value?.ToString();
            if (string.IsNullOrEmpty(cellValue))
            {
                throw new ArgumentException($"The cell at row {row} and column {columnName} is empty");
            }

            if (Enum.TryParse(cellValue, true, out TEnum result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"The value '{cellValue} in row {row} column {columnName} is not valid.");
            }
        }
        throw new KeyNotFoundException($"The column '{columnName}' does not existing in the header mapping");
    }

    public Rank GetRankColumnValue(int row, string columnName)
    {
        excelHeaders.TryGetValue(columnName.Replace(" ", "").ToLower(), out int columnNumber);
        return (Rank)worksheet.Cells[row, columnNumber].Value;
    }

    private bool CheckForResultOnRow(int row)
    {
        int CustomerId = GetIntColumnValue(row, nameof(TestResult.CustomerId));
        if (CustomerId == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private TestResult AddResult(int row)
    {
        TestResult testResult = new();
        testResult.CustomerId = GetIntColumnValue(row, nameof(TestResult.CustomerId));
        testResult.StudentId = GetIntColumnValue(row, nameof(TestResult.StudentId));
        testResult.TestId = GetIntColumnValue(row, nameof(TestResult.TestId));
        testResult.Subject = GetStringColumnValue(row, nameof(TestResult.Subject));
        testResult.RawScore = GetIntColumnValue(row, nameof(TestResult.RawScore));
        testResult.PercentCorrect = GetIntColumnValue(row, nameof(TestResult.PercentCorrect));
        testResult.RitScore = GetIntColumnValue(row, nameof(TestResult.RitScore));
        testResult.HewittPercentile = GetIntColumnValue(row, nameof(TestResult.HewittPercentile));
        testResult.NationalPercentile = GetIntColumnValue(row, nameof(TestResult.NationalPercentile));
        testResult.OverallRank = GetStringColumnValue(row, nameof(TestResult.OverallRank));
        testResult.FirstGoalRank = GetStringColumnValue(row, nameof(TestResult.FirstGoalRank));
        testResult.SecondGoalRank = GetStringColumnValue(row, nameof(TestResult.SecondGoalRank));
        testResult.ThirdGoalRank = GetStringColumnValue(row, nameof(TestResult.ThirdGoalRank));
        testResult.FourthGoalRank = GetStringColumnValue(row, nameof(TestResult.FourthGoalRank));
        testResult.FifthGoalRank = GetStringColumnValue(row, nameof(TestResult.FifthGoalRank));
        testResult.SixthGoalRank = GetStringColumnValue(row, nameof(TestResult.SixthGoalRank));
        testResult.SeventhGoalRank = GetStringColumnValue(row, nameof(TestResult.SeventhGoalRank));
        testResult.Response = GetStringColumnValue(row, nameof(TestResult.Response)) ?? string.Empty;
        testResult.Held = GetStringColumnValue(row, nameof(TestResult.Held));

        return testResult;
    }
}