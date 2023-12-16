using Microsoft.EntityFrameworkCore;

namespace BANKPROJECT8.Models
{
    public class BankDBC : DbContext
    {
        // properties for collection objects
        public ICollection<Account> Accounts_Col { get; set; }
        public ICollection<Beneficiary> Beneficiaries_Col { get; set; }
        public ICollection<Transaction> Transactions_Col { get; set; }

        public BankDBC(DbContextOptions<BankDBC> options) : base(options)
        {
            // Initialize collections
            Accounts_Col = new List<Account>();
            Beneficiaries_Col = new List<Beneficiary>();
            Transactions_Col = new List<Transaction>();
        }

        // DbSet for each entity --- tables data stored in these (names on which datasets get created in the SQL db)

        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Beneficiary>? Beneficiaries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction>? Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure precision and scale for the Balance property
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2); // Adjust precision and scale based on your requirements

            modelBuilder.Entity<Transaction>()
               .Property(a => a.Amount)
               .HasPrecision(18, 2); // Adjust precision and scale based on your requirements


            // Other configurations can be added here using Fluent API

            base.OnModelCreating(modelBuilder);
        }
    }
}
