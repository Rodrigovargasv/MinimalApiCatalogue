using ApiCatalogue.Data;
using ApiCatalogue.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

// Seeking all category
app.MapGet("/category", async (AppCatalogueDBContext db) => await db.Categorys.ToListAsync());

// Seeking category by id
app.MapGet("/catgory/{id}", async (AppCatalogueDBContext db, int id) =>
{
    return await db.Categorys.FindAsync(id) is Category category ? Results.Ok(category) : Results.NotFound();

});

app.MapPut("/category/{id}", async (AppCatalogueDBContext db, int id, Category category) =>
{
    var value = await db.Categorys.FindAsync(id);

    if (value is null) return Results.NotFound();

    value.Name = category.Name;
    
    value.Description = category.Description;

    await db.SaveChangesAsync();

    return Results.NoContent();
    
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
