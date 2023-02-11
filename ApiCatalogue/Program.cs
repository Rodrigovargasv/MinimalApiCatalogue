using ApiCatalogue.Data;
using ApiCatalogue.Models;
using ApiCatalogue.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiCatalogo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});

// Add service Entity Framework
builder.Services.AddDbContext<AppCatalogueDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AppCatalogueDBContext")));

// ADD service Jwt token
builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,

                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey
                         (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                     };
                 });

builder.Services.AddAuthorization();

var app = builder.Build();


// Endpoint to login

app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
{
    if (userModel == null)
    {
        return Results.BadRequest("Login Inválido");
    }
    if (userModel.Username == "rodrigo" && userModel.Password == "87053277")
    {
        var tokenString = tokenService.GenerationToken(app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"],
            app.Configuration["Jwt:Audience"],
            userModel);
        return Results.Ok(new { token = tokenString });
    }
    else
    {
        return Results.BadRequest("Login Inválido");
    }
}).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Autenticacao");


// Define  the endpoints

app.MapGet("/", () => "Welcome to ApiCatalogue").RequireAuthorization();

// Creates new category
app.MapPost("/category", async(AppCatalogueDBContext db, Category category) =>
{
    db.Categorys.Add(category);

    await db.SaveChangesAsync();

    return Results.Created($"/category/{category.Id}", category);
});

// Seeking all category
app.MapGet("/category", async (AppCatalogueDBContext db) => await db.Categorys.ToListAsync()).RequireAuthorization();

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

#region Endpoints the Product

//  Creates new product
app.MapPost("/product", async(AppCatalogueDBContext db, Product product) => 
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/product/{product.Id}", product);
});


app.MapGet("/product", async (AppCatalogueDBContext db) => await db.Products.ToListAsync());

app.MapGet("/product/{id}", async (AppCatalogueDBContext db, int id) =>
{
    return await db.Products.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound();
});


app.MapPut("/product/{id}", async (AppCatalogueDBContext db, Product product, int id) =>
{
    var productDB = await db.Products.FindAsync(id);

    if (product is null) return Results.NotFound();

    productDB.Name = product.Name;
    productDB.Description = product.Description;
    productDB.Category = product.Category;
    productDB.CategoryId = id;
    productDB.Stock = product.Stock;
    productDB.Price = product.Price;
    
    await db.SaveChangesAsync();

    return Results.NoContent();
    
});



app.MapDelete("/product/{id}", async (AppCatalogueDBContext db, int id) =>
{
    var product = await db.Products.FindAsync(id);

    if(product is null) return Results.NotFound();

    db.Products.Remove(product);

    await db.SaveChangesAsync();

    return Results.Ok();


});

#endregion
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Actived  services the Jwt token
app.UseAuthentication();
app.UseAuthorization();

app.Run();
