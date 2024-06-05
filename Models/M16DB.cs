using Microsoft.EntityFrameworkCore;

namespace AlignAPI.Models
{
    public class M16DB : DbContext
    {
        public M16DB(DbContextOptions<M16DB> options) : base(options) { }

        public DbSet<Mission> Missions { get; set; }
    }
}
