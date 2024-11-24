namespace passapi.models;

public class Student
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string DOB { get; set; }
    public List<TestResult> TestResults { get; set; } = new List<TestResult>();
}