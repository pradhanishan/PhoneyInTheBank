using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using PhoneyInTheBank.Models;

namespace DataContext.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<Loan> Loan { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }

        public DbSet<TransactionHistory> TransactionHistory { get; set; }

        public DbSet<Present> Present { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BankAccount>().ToTable("BankAccounts");
            modelBuilder.Entity<Loan>().ToTable("Loans");
            modelBuilder.Entity<TransactionType>().ToTable("TransactionTypes");
            modelBuilder.Entity<TransactionHistory>().ToTable("TransactionHistories");
            modelBuilder.Entity<Present>().ToTable("Presents");
        }
    }
}