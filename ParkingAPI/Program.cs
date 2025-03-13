using Microsoft.EntityFrameworkCore;
using ParkingAPI.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.OpenApi.Models;
using ParkingAPI.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ParkingContext>(options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging());
builder.Services.AddDbContext<ParkingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ParkingContext>()
    .AddDefaultTokenProviders();

var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:secret"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer (options =>
{
    options.Authority = "https://localhost:5156";
    options.Audience = "https://localhost:5156";
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("33df9570a04b4298dc2da053a50b4966a0c0e1aaea73b86bf58b63862af09c6df5e16afc4487658b2794d6b28c7c27adb49c4298b803b9e39c54f3598d6c31c7af13ac06d3e750b98cd00ba3c31500a6fd1b022994b68df4bb461d5227382353dd141ae41039d0f26ab372b167f26348b9b8a35c21fd2228503b9854d3ccc1f53de54531c3bf725bee887af65f4c91f22bed7347cd81634655cc701a823ecc970864f8035b40d17fbd794095068ea7274297b6426783af445badfaf262e6e3c7bcbeeb7dff0bfa2ceaaf7315c4f8ca9ab49e024be13ba71cf3548298412041ea59d656624c452306625df083ab7f5577396dfea003c681a5e800918f2228718c"))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ParkingAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Podaj token JWT."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddCors(options => options.AddPolicy("Cors", builder =>
{
    builder.AllowAnyOrigin().
    AllowAnyMethod().
    AllowAnyHeader();
  }
 ));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
