using System;
using CoreWars.Data;
using CoreWars.Data.Entities;

namespace CoreWars.Data.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=127.0.0.1;Port=5432;Database=CoreWars;Username=pawel;Password=pawel;";
            using (var context = new CoreWarsDataContext(connectionString))
            {
                context.Database.EnsureCreated();
                context.Add(new GameScript() {Id = Guid.NewGuid()});

                context.SaveChanges();
            }
        }
    }
}