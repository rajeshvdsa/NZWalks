using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Data
{
    public class NZWalksDbContext : DbContext
    {
        public NZWalksDbContext(DbContextOptions<NZWalksDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //seed data for difficulties easy, mediam, hard

            var difficulties = new List<Difficulty>()
            {
                new Difficulty()
                {
                    Id = Guid.Parse("ca1be042-0ace-4488-9a0e-142e088dc12b") ,
                    Name = "Easy"
                },
                new Difficulty()
                {
                    Id = Guid.Parse("1ccd76b1-c2ac-4503-8009-3c1529cf2fc8"),
                    Name = "Medium"
                },
                new Difficulty()
                {
                    Id = Guid.Parse("460e81b0-6521-4375-afa5-004fa08a2327"),
                    Name = "Hard"
                }

            };

            modelBuilder.Entity<Difficulty>().HasData(difficulties);


            var regions = new List<Region>()
            {
                new Region
                {
                    Id = Guid.Parse("8bf6549d-2dce-4f40-9067-098f238a311d"),
                    Name = "Auckland",
                    Code = "AKL",
                    RegionImageUrl = null
                },
                new Region
                {
                    Id = Guid.Parse("2ba17a97-e461-4786-bec3-9d7ea463728e"),
                    Name = "Northland",
                    Code = "NTL",
                    RegionImageUrl = null
                },
                new Region
                {
                    Id = Guid.Parse("90f2dc05-0fa6-41c8-bb09-d5ec72a8fbd6"),
                    Name = "Bay Of Plenty",
                    Code = "BOP",
                    RegionImageUrl = null
                }
            };

            modelBuilder.Entity<Region>().HasData(regions);
        }
    }
}
