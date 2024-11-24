namespace passapi.models;

public class DataUploadResponse
{
    public int TotalRowsProcessed { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public IList<string> Errors = new List<string>();
    public IList<TestResult> Data = new List<TestResult>();
 }