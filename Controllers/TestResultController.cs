using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using passapi.data;
using passapi.Interfaces;
using passapi.models;

namespace passapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestResultController : ControllerBase
{
    private readonly ITestResultUploadService _uploadService;
    private readonly AppDbContext _context;

    public TestResultController(AppDbContext context, ITestResultUploadService testResultUploadService)
    {
        _context = context;
        _uploadService = testResultUploadService;
    }

    // GET: api/TestResults
    [HttpGet]
    public async Task<List<TestResult>> GetTestResults()
    {
        return await _context.TestResults.OrderBy(x => x.CustomerId).ThenBy(x => x.TestId).ToListAsync();
    }

    // GET: api/TestResults/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<List<TestResult>>> GetTestResult(int id)
    {
        List<TestResult> TestResult = await _context.TestResults.Where(x => x.TestId == id).ToListAsync();

        if (TestResult == null)
        {
            return NotFound();
        }

        return TestResult;
    }

    // POST: api/TestResults
    [HttpPost]
    public async Task<JsonResult> CreateTestResult(IFormFile file) 
    {
        return await _uploadService.Upload(file);
    }

    // PATCH: api/TestResults/{id}
    [HttpPatch]
    public async Task<IActionResult> UpdateItem(Guid id, TestResult item)
    {
        if (id != item.Id)
        {
            return BadRequest("Item ID mismatch.");
        }

        TestResult? TestResult = await _context.TestResults.FindAsync(id);
        Console.WriteLine(TestResult);
        // _context.Entry(item).State = EntityState.Modified;

        // try
        // {
        //     await _context.SaveChangesAsync();
        // }
        // catch (DbUpdateConcurrencyException)
        // {
        //     if (!ItemExists(id))
        //     {
        //         return NotFound();
        //     }
        //     else
        //     {
        //         throw;
        //     }
        // }

        return NoContent();
    }

    // DELETE: api/TestResults/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTestResult(Guid id)
    {
        TestResult? TestResult = await _context.TestResults.FindAsync(id);
        if (TestResult == null)
        {
            return NotFound();
        }

        _context.TestResults.Remove(TestResult);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
