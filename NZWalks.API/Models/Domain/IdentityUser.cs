using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Models.Domain
{
    public class NZWalksUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
