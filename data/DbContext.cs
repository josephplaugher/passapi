
using Microsoft.EntityFrameworkCore;
using passapi.models;

namespace passapi.data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TestResult> TestResults { get; set; }
    // public DbSet<Rank> Ranks { get; set; }
    // public DbSet<Subject> Subjects { get; set; }
}