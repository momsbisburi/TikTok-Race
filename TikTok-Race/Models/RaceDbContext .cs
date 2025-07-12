using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTok_Race.Models
{
    public class RaceDbContext : DbContext
    {
        public RaceDbContext() : base(@"Server=localhost\SQLEXPRESS;Database=TikTokRaceDB2;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;") // Define this in your Web.config
        {
        }

        public DbSet<Race> Races { get; set; }
        public DbSet<PlayerWin> PlayerWins { get; set; }
        public DbSet<Player> Player { get; set; }
    }
}
