using Microsoft.EntityFrameworkCore;

namespace SmartBank.Models
{
    public class EFCoreDbContext : DbContext
    {
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        //public DbSet<Payment> Payment { get; set; } 

        public DbSet<DebitCard> DebitCards { get; set; } 

        public DbSet<CreditCard> CreditCards { get; set; }

        public DbSet<Checkbook> Checkbooks { get; set; } 

        public DbSet<Asset> Asset { get; set; } 

        public DbSet<AdminRequest> AdminRequests { get; set; }

        public DbSet<Loan> Loan{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Transaction>()
            //    .HasOne(t => t.User)
            //    .WithMany(u => u.Transactions)
            //    .HasForeignKey(t => t.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Payment>()
            //    .HasOne(p => p.User)
            //    .WithMany(u => u.Payments)
            //    .HasForeignKey(p => p.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assets)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AdminRequest>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AdminRequests)
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
