using Microsoft.EntityFrameworkCore;

namespace KR3.PaymentService.Models
{
    public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<InboxMessage> InboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.Property(e => e.Version).IsConcurrencyToken();
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.AccountId);
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Data).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.HasIndex(e => e.ProcessedUtc);
            });

            modelBuilder.Entity<InboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.MessageId).IsUnique();
                entity.Property(e => e.Data).IsRequired();
                entity.HasIndex(e => e.ProcessedUtc);
            });
        }
    }
}
