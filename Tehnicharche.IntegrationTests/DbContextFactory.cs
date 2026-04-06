using Microsoft.EntityFrameworkCore;
using Tehnicharche.Data;

namespace Tehnicharche.Tests.Integration.Helpers
{
    public static class DbContextFactory
    {
        public static TehnicharcheDbContext Create(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<TehnicharcheDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new TehnicharcheDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}