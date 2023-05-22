using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pokemon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokemon.Tests.Repository
{
    public class PokemonRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        }
    }
}
