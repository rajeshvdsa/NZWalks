using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.API.Tests
{
    public class NZWalksDbContextTests
    {
        public NZWalksDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<NZWalksDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new NZWalksDbContext(options);
        }

        [Fact]
        public void Constructor_Should_Create_Context()
        {
            //Arrange & Act
            var context = CreateDbContext();

            Assert.NotNull(context);
        }

        [Fact]
        public void Should_Have_Seeded_Difficulties()
        {
            //Arrange
            using var context = CreateDbContext();

            //Act
            context.Database.EnsureCreated();
            var difficulties = context.Difficulties.ToList();

            //Assert
            Assert.Equal(3, difficulties.Count);

            Assert.Contains(difficulties, x => x.Name == "Easy");

            Assert.Contains(difficulties, x => x.Name == "Medium");

            Assert.Contains(difficulties, x => x.Name == "Hard");
        }

        [Fact]
        public void Should_Have_Seeded_Regions()
        {
            //Arrange
            using var context = CreateDbContext();

            //ACT
            context.Database.EnsureCreated();   
            var regions = context.Regions.ToList();

            //Assert
            Assert.Equal(3, regions.Count);
            Assert.Contains(regions, x => x.Name == "Northland");
            Assert.Contains(regions, x => x.Code == "BOP");

        }

    }
}
