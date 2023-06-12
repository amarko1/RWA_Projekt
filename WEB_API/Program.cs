using DATA_LAYER.DALModels;
using DATA_LAYER.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WEB_API.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RwaMoviesContext>(options => {
    options.UseSqlServer("Name = ConnectionStrings:DefaultConnection");
});

//Configure JWT services
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
     {
         var jwtKey = builder.Configuration["JWT:Key"];
         var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
         o.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidIssuer = builder.Configuration["JWT:Issuer"],
             ValidateAudience = true,
             ValidAudience = builder.Configuration["JWT:Audience"],
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
             ValidateLifetime = true,
         };
     });

builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
