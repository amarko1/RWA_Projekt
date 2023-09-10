using DATA_LAYER.DALModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<RwaMoviesContext>(options => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddAutoMapper(typeof(AutoMapProfile));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Tag}/{action=AllTags}/{id?}");



app.Run();
