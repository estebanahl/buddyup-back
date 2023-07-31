using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using buddyUp.Models;

namespace buddyUp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<Match> Match { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Tag>()
                .HasMany(t => t.UsersWithInterest)
                .WithMany(p => p.tags);
            builder.Entity<Tag>().Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Entity<Profile>().Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Entity<Photo>().Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Entity<Match>().Property(m => m.id).ValueGeneratedOnAdd();
            builder.Entity<Message>().Property(m => m.mId).ValueGeneratedOnAdd();

            //builder.Entity<Profile>().HasIndex(p => p.UserId).IsUnique();
        }
    }
}
