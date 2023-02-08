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

// Creates new category
app.MapPost("/category", async(AppCatalogueDBContext db, Category category) =>
{
    db.Categorys.Add(category);

    await db.SaveChangesAsync();

    return Results.Created($"/category/{category.Id}", category);
});

// Seeking all category
app.MapGet("/category", async (AppCatalogueDBContext db) => await db.Categorys.ToListAsync());

// Seeking category by id
app.MapGet("/category/{id}", async (AppCatalogueDBContext db, int id) =>
{
    return await db.Categorys.FindAsync(id) is Category category ? Results.Ok(category) : Results.NotFound();

});


// Update category
app.MapPut("/category/{id}", async (AppCatalogueDBContext db, int id, Category category) =>
{
    var categoryDB = await db.Categorys.FindAsync(id);

    if (categoryDB is null) return Results.NotFound();

    categoryDB.Name = category.Name;
    
    categoryDB.Description = category.Description;

    await db.SaveChangesAsync();

    return Results.NoContent();
    
});

// Delele category
app.MapDelete("/category/{id}", async(AppCatalogueDBContext db, int id) =>
{
    if (await db.Categorys.FindAsync(id) is Category category)
    {
        db.Categorys.Remove(category);

        await db.SaveChangesAsync();

        return Results.Ok();

    }
    return Results.NotFound();
});

//  Creates new product
app.MapPost("/product", async(AppCatalogueDBContext db, Product product) => 
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/product/{product.Id}", product);
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.Run();
