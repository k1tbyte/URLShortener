
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using URLShortener.Infrastructure.Data.Context;
using URLShortener.Server.Tools;

namespace URLShortener.Server;

public static class Program
{
    private static WebApplication _app = null!;
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        EnvSettingsMapper.MapEnvToConfig(builder.Configuration);

        builder.Services.AddControllersWithViews().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
            
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddHttpContextAccessor();
        
        
        builder.Services.AddDbContext<AppDbContext>();
        

        ConfigureAuthentication(builder);
        
        _app = builder.Build();

        _app.UseDefaultFiles();
        _app.UseStaticFiles();
        _app.MapStaticAssets();

        // Configure the HTTP request pipeline.
        if (_app.Environment.IsDevelopment())
        {
            _app.MapOpenApi();
        }

        _app.UseRouting(); 
        _app.UseAuthorization();


        _app.MapRazorPages();
        _app.MapControllers();
        _app.MapFallbackToFile("/index.html");

        _app.Run();
    }
    
    
    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer              = _app.Configuration["JwtSettings:Issuer"],
                IssuerSigningKey         = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_app.Configuration["JwtSettings:Key"]!)),
                ValidateIssuer           = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime         = false,
            };
            
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = (context) =>
                {
                    var path = context.HttpContext.Request.Path;
                    if (!path.StartsWithSegments("/hubs"))
                    {
                        return Task.CompletedTask; 
                    }

                    context.Token = context.Request.Query["access_token"];
                    
                    if (string.IsNullOrEmpty(context.Token))
                    {
                        var authHeader = context.Request.Headers.Authorization.ToString();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                        {
                            context.Token = authHeader.Substring("Bearer ".Length);
                        }
                    }
                    
                    return Task.CompletedTask;
                }
            };
        });
    }

}