using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TikTok_Race.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace BlazorApp1.Data
{
    public class RaceService
    {
        private readonly IDbContextFactory<RaceDbContext> _contextFactory;

        public RaceService(IDbContextFactory<RaceDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private string NormalizePlayerName(string playerName)
        {
            if (string.IsNullOrEmpty(playerName)) return playerName;

            // Remove invisible characters and normalize
            return playerName
                .Normalize(NormalizationForm.FormC)
                .Trim()
                .Replace("\u200B", "") // Remove zero-width space
                .Replace("\u200C", "") // Remove zero-width non-joiner
                .Replace("\u200D", ""); // Remove zero-width joiner
        }
        public async Task<List<PlayerLeaderboard>> GetLeaderboardAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var playerWins = await context.PlayerWins.ToListAsync(); // Get all data first

            return playerWins
                .GroupBy(p => p.PlayerName.Normalize(NormalizationForm.FormC)) // Normalize Unicode
                .Select(g => new PlayerLeaderboard
                {
                    PlayerName = g.First().PlayerName, // Use original name for display
                    TotalPoints = g.Sum(p => p.Points)
                })
                .OrderByDescending(p => p.TotalPoints)
                .ToList(); // Remove Async - this is now in-memory
        }

        public async Task<Player> GetPlayerProfileAsync(string uniqueId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Players.FirstOrDefaultAsync(x => x.nickname == uniqueId);
        }
    }

    public class PlayerLeaderboard
    {
        public string PlayerName { get; set; }
        public int TotalPoints { get; set; }
    }

    public class RaceDbContext : DbContext
    {
        public RaceDbContext(DbContextOptions<RaceDbContext> options) : base(options) { }

        public DbSet<Race> Races { get; set; }
        public DbSet<PlayerWin> PlayerWins { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerWin>()
                .HasOne(pw => pw.Race)
                .WithMany(r => r.PlayerWins)
                .HasForeignKey(pw => pw.RaceID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class Player
    {
        [Key]
        public int PlayerID { get; set; }
        public string uniqueId { get; set; }
        public string nickname { get; set; }
        public string ProfilePicture { get; set; }
    }

    public class Race
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RaceID { get; set; } // Unique race identifier

        public DateTime RaceDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<PlayerWin> PlayerWins { get; set; }
    }

    public class PlayerWin
    {
        [Key]
        public int PlayerWinID { get; set; }

        [ForeignKey("Race")]
        public int RaceID { get; set; }
        public virtual Race Race { get; set; }

        public string PlayerName { get; set; }
        public int Placement { get; set; } // 1st, 2nd, 3rd
        public int Points { get; set; } // 3, 2, 1 points based on placement
    }
}
