using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Data
{
    public class IdentityAuthDbContext : IdentityDbContext<NZWalksUser, NZWalksRole, Guid>
    {
        public IdentityAuthDbContext(DbContextOptions<IdentityAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Create some default roles
            var readerRoleId = new Guid("00000000-0000-0000-0000-000000000001");
            var writerRoleId = new Guid("00000000-0000-0000-0000-000000000002");

            var roles = new List<NZWalksRole>
            {
                new NZWalksRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId.ToString(),
                    Name = "Reader",
                    NormalizedName = "READER"
                },
                new NZWalksRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId.ToString(),
                    Name = "Writer",
                    NormalizedName = "WRITER"
                }
            };

            builder.Entity<NZWalksRole>().HasData(roles);
        }
    }
}
