using CoreWars.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreWars.Data
{
    public interface IDataContext : IBaseRepository
    {
        DbSet<Script> Scripts { get; set; }
        DbSet<Entities.Competition> Competitions { get; set; }
        DbSet<Language> Languages { get; }
        DbSet<ScriptStatistics> Stats { get; }
        DbSet<User> Users { get; }
        DbSet<ScriptFailure> Failures { get; }
    }
    

    public interface IBaseRepository
    {
        void Commit();
    }

    public class CoreWarsDataContext : DbContext, IDataContext
    {
        private readonly string _connectionString;

        public CoreWarsDataContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Script>()
                .HasOne(x => x.Language)
                .WithMany(x => x.Scripts)
                .HasForeignKey(x => x.ScriptType);
            
            modelBuilder.Entity<Script>()
                .HasOne(x => x.Competition)
                .WithMany(x => x.Scripts)
                .HasForeignKey(x => x.CompetitionName);

            modelBuilder.Entity<ScriptStatistics>()
                .HasOne(x => x.Script)
                .WithOne(x => x.Stats);
            
            modelBuilder.Entity<ScriptFailure>()
                .HasOne(x => x.Script)
                .WithOne(x => x.FailureInfo);

            modelBuilder.Entity<User>()
                .HasMany(x => x.Scripts)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);
        
        public DbSet<Script> Scripts { get; set; }
        public DbSet<Entities.Competition> Competitions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<ScriptStatistics> Stats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ScriptFailure> Failures { get; set; }

        public void Commit()
        {
            base.SaveChanges();
        }
    }
}