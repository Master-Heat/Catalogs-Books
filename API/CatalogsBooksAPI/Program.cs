using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Services.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Identity.Core;
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
});

// 3. Keep your Controller support
builder.Services.AddControllers();

builder.Services.AddScoped<IAccountFactory, AccountFactory>();

builder.Services.AddDbContext<CatalogsBooksContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("somee"));
});


var app = builder.Build();






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

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};
//
//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
