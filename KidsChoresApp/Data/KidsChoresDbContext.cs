using KidsChoresApp.Models;
using Microsoft.EntityFrameworkCore;


namespace KidsChoresApp.Data
{
    public class KidsChoresDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Chore> Chores { get; set; }


        public KidsChoresDbContext(DbContextOptions<KidsChoresDbContext> options) : base(options)
        {
        }

        public KidsChoresDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Parent)
                .WithOne(p => p.User)
                .HasForeignKey<Parent>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Child>()
                .HasOne(c => c.User)
                .WithMany(u => u.Children)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Chore>()
                .HasOne(c => c.Child)
                .WithMany(ch => ch.Chores)
                .HasForeignKey(c => c.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Parent>()
                .HasIndex(p => p.UserId);

            modelBuilder.Entity<Child>()
                .HasIndex(c => c.UserId);

            modelBuilder.Entity<Chore>()
                .HasIndex(c => c.ChildId);
        }
    }
}
