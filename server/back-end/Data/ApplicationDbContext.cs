using Microsoft.EntityFrameworkCore;
using back_end.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Host = back_end.Models.Host;
using User = back_end.Models.User;


public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Host> Hosts { get; set; }
    public DbSet<DisabilityTag> DisabilityTags => Set<DisabilityTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring the relationship between User and Saved Events (Many-to-Many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.SavedEvents)
            .WithMany(e => e.UsersWhoSaved)
            .UsingEntity<Dictionary<string, object>>(
                "UserSavedEvents",
                j => j.HasOne<Event>().WithMany().HasForeignKey("EventId")
                      .OnDelete(DeleteBehavior.NoAction), // Prevent cascade delete here
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
                      .OnDelete(DeleteBehavior.NoAction) // Prevent cascade delete here
            );

        // Configuring the relationship between Event and Host (One-to-Many)
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Host)         // An Event has one Host
            .WithMany(h => h.Events)     // A Host has many Events
            .HasForeignKey(e => e.HostId) // Foreign Key is HostId in Event
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete events if host is deleted

        // Configuring the relationship between User and Host (One-to-One)
        modelBuilder.Entity<Host>()
            .HasOne(h => h.User)
            .WithOne()
            .HasForeignKey<Host>(h => h.Id)
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete host if user is deleted
        
        // Many-to-many: Event <-> DisabilityTag
        modelBuilder.Entity<Event>()
            .HasMany(e => e.DisabilityTags)
            .WithMany(t => t.Events)
            .UsingEntity(j => j.ToTable("EventDisabilityTags"));
        
        // Enforce unique tag names (case-insensitive) via NormalizedName
        modelBuilder.Entity<DisabilityTag>()
            .HasIndex(t => t.NormalizedName)
            .IsUnique();

        // Store enums as strings if you prefer readability
        modelBuilder.Entity<Event>().Property(e => e.Status).HasConversion<string>();
        modelBuilder.Entity<Event>().Property(e => e.Location).HasConversion<string>();
    }
}

