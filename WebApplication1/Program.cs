using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.   
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// MongoDB config
var mongoConnectionString = "mongodb+srv://rakeed:02022708xzp@horrstaf.re0hh.mongodb.net/?retryWrites=true&w=majority&appName=HorrStaf";
var mongoDbName = "HorrStaf";
builder.Services.AddSingleton(new MongoDbService(mongoConnectionString, mongoDbName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bron}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "test",
    pattern: "test",
    defaults: new { controller = "Test", action = "Index" });

app.Run();