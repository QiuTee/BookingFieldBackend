using Microsoft.EntityFrameworkCore;
using FieldBookingAPI.Models;

namespace FieldBookingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingSlot> BookingSlots => Set<BookingSlot>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<FieldImage> FieldImages => Set<FieldImage>();
        public DbSet<FieldService> FieldServices => Set<FieldService>();
        public DbSet<FieldReview> FieldReviews => Set<FieldReview>();
        public DbSet<SubField> SubFields => Set<SubField>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .IsRequired(false);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Slots)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasDefaultValue("unpaid");

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingCode)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Field)
                .WithMany()
                .HasForeignKey(b => b.FieldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .Property(b => b.IsRead)
                .HasDefaultValue(false);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Voucher)
                .WithMany()
                .HasForeignKey(b => b.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Field>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.Fields)
                .HasForeignKey(f => f.OwnerId);

            modelBuilder.Entity<Field>()
                .HasOne(f => f.CreatedByAdmin)
                .WithMany()
                .HasForeignKey(f => f.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Field>()
                .HasMany(f => f.Images)
                .WithOne(fi => fi.Field)
                .HasForeignKey(fi => fi.FieldId);

            modelBuilder.Entity<Field>()
                .HasMany(f => f.Services)
                .WithOne(fs => fs.Field)
                .HasForeignKey(fs => fs.FieldId);

            modelBuilder.Entity<Field>()
                .HasMany(f => f.Reviews)
                .WithOne(fr => fr.Field)
                .HasForeignKey(fr => fr.FieldId);

            modelBuilder.Entity<Field>()
                .HasMany(f => f.SubFields)
                .WithOne(sf => sf.Field)
                .HasForeignKey(sf => sf.FieldId);
        }
    }
}
