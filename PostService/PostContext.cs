using Microsoft.EntityFrameworkCore;

public class PostContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public PostContext(DbContextOptions<PostContext> options) : base(options)
    {
    }
}
