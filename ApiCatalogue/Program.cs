using ApiCatalogue.Data;
using ApiCatalogue.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add service Entity Framework
builder.Services.AddDbContext<AppCatalogueDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AppCatalogueDBContext")));


var app = builder.Build();

// Define  the endpoints

app.MapGet("/", () => "Welcome to ApiCatalogue");

app.MapPost("/category", async(Category category, AppCatalogueDBContext db) =>
{
    db.Categorys.Add(category);
    await db.SaveChangesAsync();

    return Results.Created($"/category/{category.Id}", category);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
