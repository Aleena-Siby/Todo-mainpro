// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
namespace WebApi.Data{
    


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> TaskItem { get; set; }  // Define your DbSets here
}
}