using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); //Controllers ve Views'leri eklemek için

builder.Services.AddDbContext<BlogContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("mysql_connection");
    //options.UseSqlite(connectionString);
    var version = new MySqlServerVersion(new Version(8, 2, 0));
    options.UseMySql(connectionString, version);
});

builder.Services.AddScoped<IPostRepository, EfPostRepository>(); //IPostRepository ve EfPostRepository arasında bağlantı kurmak için injection yapılır.
//Her IPostRepository isteği geldiğinde EfPostRepository kullanılacak.
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => //Cookie tabanlı kimlik doğrulama eklemek için
{
    options.LoginPath = "/Users/Login"; //Authorize olmamış kullanıcıları otomatik olarak yönlendirmek için

});

var app = builder.Build();

app.UseStaticFiles(); //wwwroot klasörüne erişim sağlamak için

app.UseRouting(); //Routing işlemlerini başlatmak için
app.UseAuthentication(); //Kimlik doğrulama işlemlerini başlatmak için
app.UseAuthorization(); //Yetkilendirme işlemlerini başlatmak için

SeedData.TestVerileriniDoldur(app);

app.MapControllerRoute(
    name: "post_details",
    pattern: "posts/details/{url}",
    defaults: new { controller = "Post", action = "Details" }
);

app.MapControllerRoute(
    name: "posts_by_tag",
    pattern: "posts/tag/{tag}",
    defaults: new { controller = "Post", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}"
);

//app.MapGet("/", () => "Hello World!");
app.Run();
