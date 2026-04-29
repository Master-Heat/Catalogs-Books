using System.Text;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using CatalogsBooksAPI.Services;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Identity.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



// 1. Add this to support API exploration
builder.Services.AddEndpointsApiExplorer();

// 2. Add this to generate the Swagger JSON
builder.Services.AddSwaggerGen(options =>
{
    // ... your other JWT/Swagger settings ...

    options.TagActionsBy(api =>
    {
        // This takes the namespace (e.g., YourProject.Controllers.Admin) 
        // and uses the last part as the heading name
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            // Option A: Use the full namespace
            // return new[] { controllerActionDescriptor.ControllerTypeInfo.Namespace };

            // Option B: Use just the last part of the namespace (e.g. "Admin" or "Inventory")
            var parts = controllerActionDescriptor.ControllerTypeInfo.Namespace.Split('.');
            return new[] { parts.Last() };
        }

        return new[] { api.RelativePath };
    });

    // This helps Swagger keep the operations sorted correctly within the new groups
    options.DocInclusionPredicate((name, api) => true);
    var JwtSecuritySchema = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter your JWT Access Token",


    };
    options.AddSecurityDefinition("Bearer", JwtSecuritySchema
        //{
        //Name = "Authorization",
        //    In = ParameterLocation.Header,
        //    Type = SecuritySchemeType.Http,
        //    Scheme = "bearer", // Lowercase "bearer" is recommended for the HTTP scheme
        //    BearerFormat = "JWT",
        //    Description = "Enter your JWT Access Token"
        //}
        );
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
{
    {
        // Fix for Error 1: Pass 'document' to the constructor
        // Fix for Error 2: Use 'new List<string>()' instead of 'Array.Empty<string>()'
        new OpenApiSecuritySchemeReference("Bearer", document),
        new List<string>()
    }
});

    // options.AddSecurityDefinition("Bearer", JwtSecuritySchema);


}
);


// 3. Keep your Controller support
builder.Services.AddControllers();

builder.Services.AddScoped<AccountFactory>();
builder.Services.AddScoped<AuthorFactory>();
builder.Services.AddScoped<BookFactory>();
builder.Services.AddScoped<BooksRecsCardListFactory>();
builder.Services.AddScoped<CategoryFactory>();
builder.Services.AddScoped<HomePageFactory>();
builder.Services.AddScoped<ReviewFactory>();

builder.Services.AddScoped<Authentication>();


builder.Services.AddScoped<AccountRepo>();
builder.Services.AddScoped<BooksRecsRepo>();



builder.Services.AddDbContext<CatalogsBooksContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("somee"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWTConfig:Key"])),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false

    };
}
);
builder.Services.AddAuthorization();



builder.Services.AddCors(options =>
{
    options.AddPolicy("policy1", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();
// app.UseStaticFiles();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("policy1");

app.Run();

