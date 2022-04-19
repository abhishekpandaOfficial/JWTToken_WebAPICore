using JWT_WebAPICore.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT_WebAPICore.Context
{
    public class ApplicationDBContext :DbContext
    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {

        }

        public virtual DbSet<User> User { get; set; }
    }
}
