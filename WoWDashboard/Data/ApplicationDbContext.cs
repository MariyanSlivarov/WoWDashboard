using Microsoft.EntityFrameworkCore;
using WoWDashboard.Models;

namespace WoWDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Character> Characters { get; set; }
        public DbSet<GearItem> GearItems { get; set; }
        public DbSet<RaidProgression> RaidProgressions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Realm).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Race).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Guild).HasMaxLength(50);
                entity.Property(e => e.CharacterClass).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RaiderIoScore).HasColumnType("float");
                entity.Property(e => e.AvatarUrl).HasMaxLength(255);

                entity.HasMany(c => c.GearItems)
                    .WithOne(g => g.Character)
                    .HasForeignKey(g => g.CharacterId);

                entity.HasOne(c => c.RaidProgression)
                    .WithOne(r => r.Character)
                    .HasForeignKey<RaidProgression>(r => r.CharacterId);
            });

            modelBuilder.Entity<GearItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Slot).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Rarity).HasMaxLength(50);
            });

            modelBuilder.Entity<RaidProgression>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RaidName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Summary).HasMaxLength(500);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}