using Microsoft.EntityFrameworkCore;
using RaceDay.Models;

namespace RaceDay.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(DbContextOptions<RaceDayContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Enrolment> Enrolments { get; set; } = null!;

        public DbSet<Event> Events { get; set;  } = null!;

        public DbSet<Result> Results { get; set; } = null!;
    
        public DbSet<EventType> EventTypes { get; set; } = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        
            // Map C# property names to the existing SQL columns.
            modelBuilder.Entity<Result>()
                .Property(r => r.Position)
                .HasColumnName("finishPosition");

            // Display properties are not columns in these tables.
            modelBuilder.Entity<Event>()
                .Property(e => e.EventType)
                .HasColumnName("eventTypeId");

            modelBuilder.Entity<Event>()
                .Property(e => e.Organiser)
                .HasColumnName("organiserId");

            modelBuilder.Entity<Event>()
                .Ignore(e => e.EventType)
                .Ignore(e => e.Organiser);

            modelBuilder.Entity<Category>()
                .Ignore(c => c.EventName);

            modelBuilder.Entity<Enrolment>()
                .Ignore(e => e.ParticipantName)
                .Ignore(e => e.Event)
                .Ignore(e => e.Category);

            modelBuilder.Entity<Result>()
                .Ignore(r => r.EventId)
                .Ignore(r => r.EventName)
                .Ignore(r => r.ParticipantName);

        }
    }
}

