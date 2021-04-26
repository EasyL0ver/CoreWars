using System;
using System.Data.Entity.Infrastructure.Interception;
using CoreWars.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreWars.Data
{
    public interface IDataContext : IBaseRepository
    {
        DbSet<GameScript> Scripts { get; set; }
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
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);
        
        public DbSet<GameScript> Scripts { get; set; }
        
        public void Commit()
        {
            base.SaveChanges();
        }
    }
}