using System.ComponentModel.DataAnnotations;

namespace passapi.models;

public class TestResult
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid StudentId { get; set; }
    public Guid TestId { get; set; }
    public Subject Subject { get; set; }
    public int RawScore { get; set; }
    public int PercentCorrect { get; set; }
    public int RitScore { get; set; }
    public int HewittPercentile { get; set; }
    public int NationalPercentile { get; set; }
    public Rank OverallRank { get; set; }
    public Rank FirstGoalRank { get; set; }
    public Rank SecondGoalRank { get; set; }
    public Rank ThirdGoalRank { get; set; }
    public Rank FourthGoalRank { get; set; }
    public Rank FifthGoalRank { get; set; }
    public Rank SixthGoalRank { get; set; }
    public Rank SeventhGoalRank { get; set; }
    public required string Response { get; set; }
    public string? Held { get; set; }
}