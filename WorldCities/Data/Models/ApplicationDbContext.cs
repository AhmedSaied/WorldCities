using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldCities.Data.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext():base()
        {

        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

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
