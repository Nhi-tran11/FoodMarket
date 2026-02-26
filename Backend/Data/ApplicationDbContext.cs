using Microsoft.EntityFrameworkCore;
using Backend.Model;
namespace Backend.Data
{
    public class ApplicationDbContext: DbContext
    { public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Customers> Customers { get; set;}
        public DbSet<Products> Products { get; set;}
        public DbSet<ShippingDetail> ShippingDetails {get; set;}
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ReferalCode> ReferalCodes { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customers>(entity =>
            {   entity.HasKey(c => c.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(p => p.Password).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasMany(c => c.ReferalCodes)
                    .WithOne(r => r.ReceivedByUser)
                    .HasForeignKey(r => r.ReceivedByUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
                entity.Property(p => p.Price).IsRequired();
                entity.Property(p => p.Category).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.Image).HasMaxLength(500);
                entity.Property(p => p.InStock).IsRequired();
                entity.Property(p => p.Unit).HasMaxLength(50);
                entity.Property(p => p.PromotionPrice);
                entity.Property(p => p.PromotionStartDate);
                entity.Property(p => p.PromotionEndDate);
            
            });
            modelBuilder.Entity<Products>()
                .Property(p=>p.RowVersion)
                .IsRowVersion();
            modelBuilder.Entity<ShippingDetail>(entity =>
            {
                entity.HasKey(s=>s.Id);
                entity.Property(s=>s.Address).IsRequired().HasMaxLength(500);
                entity.Property(s=>s.FullName).IsRequired().HasMaxLength(100);
                entity.Property(s=>s.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(s=>s.City).IsRequired().HasMaxLength(100);
                entity.Property(s=>s.State).IsRequired().HasMaxLength(100);
                entity.Property(s=>s.ZipCode).IsRequired().HasMaxLength(20);
                entity.Property(s=>s.Country).IsRequired().HasMaxLength(100);
                entity.Property(s=>s.IsDefault).IsRequired();
                entity.Property(s=>s.CreatedAt).IsRequired();
                entity.HasOne(s=>s.Customer)
                        .WithMany(c=>c.ShippingDetails)
                        .HasForeignKey(s=>s.CustomerId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p=>p.Id);
                entity.Property(p => p.OrderId).IsRequired();
                entity.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(p => p.StripePaymentIntentId).IsRequired().HasMaxLength(255);
                entity.Property(p => p.Status).IsRequired().HasMaxLength(50);
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.Currency).IsRequired().HasMaxLength(10);     
                
                    
            });
            modelBuilder.Entity<ReferalCode>(entity =>
            { 
                entity.HasKey(s=>s.Id);
                entity.Property(s => s.Code).IsRequired().HasMaxLength(50);
                entity.Property(s => s.CreatedAt).IsRequired();
                entity.HasIndex(s=>s.Code).IsUnique();
                entity.Property(s => s.IsUsed).IsRequired();
                //Referral code -> Inviter who created it (will get reward)
                entity.HasOne(s =>s.Referrer)
                    .WithMany()
                    .HasForeignKey(r => r.RefererId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.UsedReferalCode)
                    .WithMany()
                    .HasForeignKey(o => o.UsedReferalCodeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Discount>(entity =>
                {
                    entity.HasKey(d => d.Id);
                    entity.Property(d => d.DiscountPercentage).IsRequired().HasColumnType("decimal(5,2)");
                    entity.Property(d => d.DiscountAmount).HasColumnType("decimal(18,2)");
                    entity.Property(d => d.Source).IsRequired().HasMaxLength(50);
                    entity.HasOne(d => d.User)
                        .WithMany()
                        .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                }
            );

        }
}
}
