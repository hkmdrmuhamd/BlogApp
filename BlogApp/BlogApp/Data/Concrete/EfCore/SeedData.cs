
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EfCore
{
    public class SeedData
    {
        internal static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if(context != null)
            {
                
                if (!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Tag() { Text = "web programlama" },
                        new Tag() { Text = "Backend" },
                        new Tag() { Text = "Frontend" },
                        new Tag() { Text = "Fullstack" },
                        new Tag() { Text = "Php" }
                    );
                    context.SaveChanges();
                }

                if(!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User() { UserName = "hkmdr" },
                        new User() { UserName = "muhammed" }
                    );
                    context.SaveChanges();
                }
                if(!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post() { 
                            Title = "Asp.Net Core", 
                            Content = "Asp.Net Core MVC dersleri", 
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(3).ToList(), 
                            UserId = 1,
                        },
                        new Post()
                        {
                            Title = "Php Core",
                            Content = "Php Core dersleri",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = context.Tags.Take(2).ToList(),
                            UserId = 1,
                        },
                        new Post()
                        {
                            Title = "Spring Boot",
                            Content = "Spring Boot dersleri",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-5),
                            Tags = context.Tags.Take(4).ToList(),
                            UserId = 2,
                        }
                    );
                    context.SaveChanges();
                }
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
