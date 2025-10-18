using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FastVocab.Infrastructure.Data.EFCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<WordList> WordLists { get; set; }
    public DbSet<WordListDetail> WordListDetails { get; set; }
    public DbSet<WordTopic> WordTopics { get; set; }
    public DbSet<PracticeSesssion> PracticeSessions { get; set; }
    public DbSet<TakedWord> TakedWords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
