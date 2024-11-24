using System.ComponentModel.DataAnnotations;

namespace passapi.models;

public class TestResult
{
    [Key]
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public int StudentId { get; set; }
    public int TestId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int TestLevel { get; set; }
    public int RawScore { get; set; }
    public int PercentCorrect { get; set; }
    public int RitScore { get; set; }
    public int HewittPercentile { get; set; }
    public int NationalPercentile { get; set; }
    public string OverallRank { get; set; } = string.Empty;
    public string FirstGoalRank { get; set; } = string.Empty;
    public string SecondGoalRank { get; set; } = string.Empty;
    public string ThirdGoalRank { get; set; } = string.Empty;
    public string FourthGoalRank { get; set; } = string.Empty;
    public string FifthGoalRank { get; set; } = string.Empty;
    public string SixthGoalRank { get; set; } = string.Empty;
    public string SeventhGoalRank { get; set; } = string.Empty;
    public string? Response { get; set; }
    public string? Held { get; set; }
}