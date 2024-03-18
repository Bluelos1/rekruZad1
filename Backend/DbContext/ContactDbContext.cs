using Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.DbContext
{
    public class ContactDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        
        public ContactDbContext(DbContextOptions<ContactDbContext> options)
            : base(options) { }

        public virtual DbSet<Contact> ContactDb { get; set; } = null!;
    }
}
