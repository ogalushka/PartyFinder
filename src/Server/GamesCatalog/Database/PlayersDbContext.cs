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

            //TODO sql can't handle cascade deleting two references that point to the same key, since it's multiple cascade paths
            modelBuilder.Entity<Invitations>()
                .HasOne(invitation => invitation.Receiver)
                .WithMany()
                .HasForeignKey(invitation => invitation.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<PlayerInfo> PlayerInfo { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerInfoGame> PlayerInfoGame { get; set; }
        public DbSet<PlayerTime> PlayerTimes { get; set; }
        public DbSet<Invitations> Invitations { get; set; }
    }

    public class PlayerInfo
    {
        public Guid Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(100)]
        public string DiscordId { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new();
        public List<PlayerTime> Times { get; set; } = new();
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

    [PrimaryKey(nameof(PlayerInfoId), nameof(StartTime), nameof(EndTime))]
    public class PlayerTime
    {
        [Key]
        [Column(Order = 2)]
        public int StartTime { get; set; }
        [Key]
        [Column(Order = 3)]
        public int EndTime { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("PlayerInfoId")]
        public Guid PlayerInfoId { get; set; }

        public PlayerInfo PlayerInfo { get; set; } = new();
    }

    [PrimaryKey(nameof(SenderId), nameof(ReceiverId))]
    public class Invitations
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public PlayerInfo Sender { get; set; } = null!;
        public PlayerInfo Receiver { get; set; } = null!;
    }
}
