using Microsoft.EntityFrameworkCore;
using Laba_13.Models;

namespace Laba_13.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
}