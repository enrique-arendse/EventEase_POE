using EventEase_POE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventEase_POE.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Venue> Venues { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<User> Users { get; set; } 

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Prevent double booking
			modelBuilder.Entity<Booking>()
				.HasIndex(b => new { b.VenueId, b.BookingDate })
				.IsUnique();

			// Prevent cascade delete
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Venue)
				.WithMany(v => v.Bookings)
				.HasForeignKey(b => b.VenueId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Event)
				.WithMany(e => e.Bookings)
				.HasForeignKey(b => b.EventId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
