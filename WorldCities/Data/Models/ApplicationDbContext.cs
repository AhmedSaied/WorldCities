using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WorldCities.Data.Models
{
    public class ApplicationDbContext: ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, 
            IOptions<OperationalStoreOptions> operationalStoreOptions) 
            : base(options, operationalStoreOptions){}

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<City>().HasKey(a => a.Id);
            modelBuilder.Entity<City>().Property(a => a.Id).IsRequired();

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Country>().HasKey(a => a.Id);
            modelBuilder.Entity<Country>().Property(a => a.Id).IsRequired();

            modelBuilder.Entity<City>().HasOne(a => a.Country).WithMany(a => a.Cities).HasForeignKey(a => a.CountryId);
        }
    }
}
