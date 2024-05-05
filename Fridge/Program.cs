using System;
using Fridge.DAL;
using Fridge.DAL.Interfaces;
using Fridge.DAL.Repositories;
using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Infrastructure.Interfaces;
using Fridge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddScoped<IBaseRepositories<ProductEntity>, ProductRepositories>()
    .AddScoped<IProductService, ProductService>()
    .AddDbContext<FridgeDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("postgres");
        options.UseNpgsql(connectionString);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapPost("api/product/create", async (IProductService productService, ProductModel createProductModel) =>
{
    var response = await productService.Create(createProductModel);

    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        return Results.Ok(new { description = response.Description });

    return Results.BadRequest(new { description = response.Description });
});

app.MapGet("api/product/get", async (IProductService productService) =>
{
    var response = await productService.GetProductsInFridge();

    return Results.Json(new { data = response.Data });
});

app.MapDelete("api/product/delete", async (IProductService productService, long id) =>
{
    var response = await productService.DeleteProductsInFridge(id);

    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        return Results.Ok(new { description = response.Description });

    return Results.BadRequest(new { description = response.Description });
});

app.MapPost("api/product/change", async (IProductService productService, ProductModel model) =>
{
    var response = await productService.ChangeProductsInFridge(model);

    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
        return Results.Ok(new { description = response.Description });

    return Results.BadRequest(new { description = response.Description });
});

app.Run();
