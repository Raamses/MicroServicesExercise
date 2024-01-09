using Microsoft.EntityFrameworkCore;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }


    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlServer("Your MSSQL connection string");
    // }
}
