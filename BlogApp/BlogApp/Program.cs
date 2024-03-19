using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
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

var app = builder.Build();

app.UseStaticFiles(); //wwwroot klasörüne erişim sağlamak için

SeedData.TestVerileriniDoldur(app);

app.MapDefaultControllerRoute(); //Default Controller Route kullanmak için

//app.MapGet("/", () => "Hello World!");
app.Run();
