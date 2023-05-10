using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamesCatalog.Database
{
    public class PlayersDbContext : DbContext
    {
        public PlayersDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerInfo>()
                .HasMany(p => p.Games)
                .WithMany(g => g.Players)
                .UsingEntity<PlayerInfoGame>();
        }

        public DbSet<PlayerInfo> PlayerInfo { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerInfoGame> PlayerInfoGame { get; set; }
    }

    public class PlayerInfo
    {
        public Guid Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(100)]
        public string DiscordId { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new();
    }

    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(150)]
        public string? CoverUrl { get; set; }

        public List<PlayerInfo> Players { get; set; } = new();
    }

    public class PlayerInfoGame
    {
        public Guid PlayerInfoId { get; set; }
        public int GameId { get; set; }
    }
}
