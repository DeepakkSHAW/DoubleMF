using DoubleMF.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace DoubleMF.Data
{


    public class DoubleMFDBContext : DbContext
    {
        //entities
        public DbSet<AssetManagtComp> assetManagtComps { get; set; }
        public DbSet<MutualFund> mutualFunds { get; set; }
        public DbSet<NetAssetValue> netAssetValues { get; set; }

        //Constructions
        public DoubleMFDBContext() : base() { }
        public DoubleMFDBContext(DbContextOptions<DoubleMFDBContext> options) : base(options)
        {
            //Database.Migrate();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (!optionsBuilder.IsConfigured)
                {
                    /*This hard coded connection only required for EFCore migration purpose (need not be required at run time)*/
                    //var dbConnection = @"Data Source=..//Data//DB//DoubleMF.db;Version=3; Password=myLitePW";
                    var dbConnection = @"Data Source=..\\..\\..\\..\\Data\\DB\\DoubleMF.db";
                    //var dbConnection = @"Data Source=DoubleMF.db";
                    //optionsBuilder.UseSqlite(dbConnection);
                     optionsBuilder.UseSqlite(dbConnection, options => options.MaxBatchSize(512));
                     optionsBuilder.EnableSensitiveDataLogging();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*declare the Primary key on each table*/
            modelBuilder.Entity<AssetManagtComp>(entity => { entity.HasKey(p => p.AMCId); });
            modelBuilder.Entity<MutualFund>(entity => { entity.HasKey(p => p.MutualFundId); });
            modelBuilder.Entity<NetAssetValue>(entity => { entity.HasKey(p => p.NetAssetValueId); });

            /*Set Default Value as current date time to inDate Field*/
            modelBuilder.Entity<AssetManagtComp>()
                  .Property(c => c.InDate)
                  //.IsRequired()
                  //.HasDefaultValueSql("getutcdate()");   //for sql server
                  .HasDefaultValueSql("CURRENT_TIMESTAMP"); //for sqlite
            /*Set Default Value as current date time to inDate Field*/
            modelBuilder.Entity<MutualFund>()
                  .Property(c => c.InDate)
                  //.IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP"); //for sqlite

            /*declare the One-2-Many relationship*/
            modelBuilder.Entity<AssetManagtComp>()
                .HasMany(m => m.MutualFunds)
                .WithOne(a => a.AMC);
            modelBuilder.Entity<MutualFund>()
                .HasMany(m => m.NetAssetValues)
                .WithOne(a => a.MF);

            /*Insert a Dummy record on table creation (optional)*/
            //modelBuilder.Entity<AssetManagtComp>().HasData(
            //    new AssetManagtComp() { AMCId = 1, AMCName = "OM Test AMC" }
            //    );

        }
    }
}
