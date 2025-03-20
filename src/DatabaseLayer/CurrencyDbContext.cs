using DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseLayer;

public class CurrencyDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<UserToCurrency> UserToCurrencies { get; set; }
    
    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Configure primary keys
        modelBuilder.UseSerialColumns();
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<Currency>().HasKey(x => x.Id);
        modelBuilder.Entity<UserToCurrency>().HasKey(x => x.Id);
        
        //Configure indexes and constrains
        modelBuilder.Entity<User>().HasIndex(p => p.Username).IsUnique();
        
        //Configure foreing keys
        modelBuilder.Entity<UserToCurrency>()
            .HasOne(x => x.User)
            .WithMany(u => u.UserToCurrencies)
            .HasForeignKey(f => f.UserId);
        
        modelBuilder.Entity<UserToCurrency>()
            .HasOne(x => x.Currency)
            .WithMany(u => u.UserToCurrencies)
            .HasForeignKey(f => f.CurrencyId);

    }
}