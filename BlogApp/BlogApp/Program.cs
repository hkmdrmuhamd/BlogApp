using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); //Controllers ve Views'leri eklemek i�in

builder.Services.AddDbContext<BlogContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("mysql_connection");
    //options.UseSqlite(connectionString);
    var version = new MySqlServerVersion(new Version(8, 2, 0));
    options.UseMySql(connectionString, version);
});

builder.Services.AddScoped<IPostRepository, EfPostRepository>(); //IPostRepository ve EfPostRepository aras�nda ba�lant� kurmak i�in injection yap�l�r.
//Her IPostRepository iste�i geldi�inde EfPostRepository kullan�lacak.

var app = builder.Build();

app.UseStaticFiles(); //wwwroot klas�r�ne eri�im sa�lamak i�in

SeedData.TestVerileriniDoldur(app);

app.MapDefaultControllerRoute(); //Default Controller Route kullanmak i�in

//app.MapGet("/", () => "Hello World!");
app.Run();
