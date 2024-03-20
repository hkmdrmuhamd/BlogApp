
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EfCore
{
    public class SeedData
    {
        internal static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if (context != null)
            {

                if (!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Tag() { Text = "web programlama", Url = "web-programlama", Color = TagColors.primary },
                        new Tag() { Text = "Backend", Url = "backend", Color = TagColors.secondary },
                        new Tag() { Text = "Frontend", Url = "frontend", Color = TagColors.success },
                        new Tag() { Text = "Fullstack", Url = "fullstack", Color = TagColors.danger },
                        new Tag() { Text = "Php", Url = "php", Color = TagColors.warning }
                    );
                    context.SaveChanges();
                }

                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User() { UserName = "hkmdr" },
                        new User() { UserName = "muhammed" }
                    );
                    context.SaveChanges();
                }
                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post()
                        {
                            Title = "Asp.Net Core",
                            Content = "Asp.Net Core MVC dersleri",
                            Url = "asp-net-core",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(3).ToList(),
                            Image = "1.jpg",
                            UserId = 1,
                        },
                        new Post()
                        {
                            Title = "Php Core",
                            Content = "Php Core dersleri",
                            Url = "php-core",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = context.Tags.Take(2).ToList(),
                            Image = "2.jpg",
                            UserId = 1,
                        },
                        new Post()
                        {
                            Title = "Spring Boot",
                            Content = "Spring Boot dersleri",
                            Url = "spring-boot",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-5),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "3.jpg",
                            UserId = 2,
                        },
                        new Post()
                        {
                            Title = "Laravel",
                            Content = "Laravel Boot dersleri",
                            Url = "laravel",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = context.Tags.Take(2).ToList(),
                            Image = "3.jpg",
                            UserId = 2,
                        },
                        new Post()
                        {
                            Title = "Django",
                            Content = "Django Boot dersleri",
                            Url = "django",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-30),
                            Tags = context.Tags.Take(3).ToList(),
                            Image = "3.jpg",
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
