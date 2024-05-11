using System.Text;
using Fridge.DAL;
using Fridge.DAL.Interfaces;
using Fridge.DAL.Repositories;
using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Infrastructure.Interfaces;
using Fridge.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Readbooru", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a token",
        Name = "Auth",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//json web token(JWT) configuration
var JwtOptionsSection = builder.Configuration.GetRequiredSection("Jwt");
var ConnectionString = builder.Configuration.GetConnectionString("postgres");

//DI + db
//TO DO. Вынести в отдельный файл весь DI
builder.Services
    .AddScoped<IBaseRepositories<ProductEntity>, ProductRepositories>()
    .AddTransient<IBaseRepositories<UserEntity>, UserRepositories>()
    .AddScoped<IProductService, ProductService>()
    .AddTransient<IUserService, UserService>()
    .AddDbContextPool<FridgeDbContext>(options =>
    {
        options.UseNpgsql(ConnectionString);
    });

//auth и говнокод
builder.Services.Configure<JwtModel>(JwtOptionsSection);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions => {
    var key = Encoding.UTF8.GetBytes(JwtOptionsSection["Key"]);

    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = JwtOptionsSection["Issuer"],
        ValidAudience = JwtOptionsSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllerRoute(
    name: "user",
    pattern: "{controller=user}"
);
app.MapControllerRoute(
    name: "product",
    pattern: "{controller=product}"
);

app.Run();
