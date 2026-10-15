using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.user_id);

                entity.Property(u => u.user_id)
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.first_name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.last_name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(u => u.email)
                    .IsUnique();

                entity.Property(u => u.password)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(u => u.role)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(u => u.created_at)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ORGANISERS
            modelBuilder.Entity<Organiser>(entity =>
            {
                entity.ToTable("Organisers");

                entity.HasKey(o => o.organiser_id);

                entity.Property(o => o.organiser_id)
                    .ValueGeneratedOnAdd();

                entity.Property(o => o.organisation_name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(o => o.user_id)
                    .IsUnique();

                entity.HasOne(o => o.User)
                    .WithOne()
                    .HasForeignKey<Organiser>(o => o.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PARTICIPANTS
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("Participants");

                entity.HasKey(p => p.participant_id);

                entity.Property(p => p.participant_id)
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.emergency_contact_name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.emergency_contact_phone)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(p => p.user_id)
                    .IsUnique();

                entity.HasOne(p => p.User)
                    .WithOne()
                    .HasForeignKey<Participant>(p => p.user_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EVENTS
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");

                entity.HasKey(e => e.event_id);

                entity.Property(e => e.event_id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.event_name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.event_date)
                    .IsRequired();

                entity.Property(e => e.location)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.description)
                    .HasMaxLength(500);

                entity.Property(e => e.distance_km)
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();

                entity.Property(e => e.event_type)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Upcoming")
                    .IsRequired();

                entity.HasOne(e => e.Organiser)
                    .WithMany()
                    .HasForeignKey(e => e.organiser_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CATEGORIES
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.HasKey(c => c.category_id);

                entity.Property(c => c.category_id)
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.category_name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.distance_km)
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();

                entity.Property(c => c.entry_fee)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0.00m)
                    .IsRequired();

                entity.HasIndex(c => new
                {
                    c.event_id,
                    c.category_name
                })
                .IsUnique();

                entity.HasOne(c => c.Event)
                    .WithMany(e => e.Categories)
                    .HasForeignKey(c => c.event_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ENROLMENTS
            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.ToTable("Enrolments");

                entity.HasKey(e => e.enrolment_id);

                entity.Property(e => e.enrolment_id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.enrolment_date)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Registered")
                    .IsRequired();

                entity.HasIndex(e => new
                {
                    e.participant_id,
                    e.category_id
                })
                .IsUnique();

                entity.HasOne(e => e.Participant)
                    .WithMany()
                    .HasForeignKey(e => e.participant_id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Enrolments)
                    .HasForeignKey(e => e.category_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RESULTS
            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("Results");

                entity.HasKey(r => r.result_id);

                entity.Property(r => r.result_id)
                    .ValueGeneratedOnAdd();

                entity.Property(r => r.finish_time)
                    .HasColumnType("time")
                    .IsRequired();

                entity.Property(r => r.position)
                    .IsRequired(false);

                entity.Property(r => r.result_status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Finished")
                    .IsRequired();

                entity.HasIndex(r => r.enrolment_id)
                    .IsUnique();

                entity.HasOne(r => r.Enrolment)
                    .WithOne(e => e.Result)
                    .HasForeignKey<Result>(r => r.enrolment_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}