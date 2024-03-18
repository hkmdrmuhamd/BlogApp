using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Concrete.EfCore
{
    public class BlogContext:DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options):base(options)
        {
        }
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<User> USers => Set<User>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Tag> Tags => Set<Tag>();
        
    }
}
