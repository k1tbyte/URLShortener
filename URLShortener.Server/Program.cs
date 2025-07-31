using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using URLShortener.Infrastructure.Data.Context;
using URLShortener.Infrastructure.Repositories;
using URLShortener.Infrastructure.Repositories.Abstraction;
using URLShortener.Infrastructure.Services;
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

        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
            
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpContextAccessor();
        
        
        
        
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUrlsRepository, UrlsRepository>();
        
        
        

        ConfigureAuthenticationAndAuthorization(builder);
        
        _app = builder.Build();

        _app.UseDefaultFiles();
        _app.UseStaticFiles();
        _app.MapStaticAssets();

        // Configure the HTTP request pipeline.
        if (_app.Environment.IsDevelopment())
        {
            _app.MapOpenApi();
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }

        _app.UseRouting(); 
        _app.UseAuthentication();
        _app.UseAuthorization();


        _app.MapRazorPages();
        _app.MapControllers();
        _app.MapFallbackToFile("/index.html");

        _app.Run();
    }
    
    
    /*static Task TokenValidatedEvent(TokenValidatedContext context)
    {
        if (context.SecurityToken.ValidTo >= DateTime.UtcNow) 
            return Task.CompletedTask;
            
        using var scope      = _app.Services.CreateScope();
        var       jwtService = scope.ServiceProvider.GetService(typeof(JwtAuthService)) as JwtAuthService;
            
        //We used only unique claims for easy to use
        var payload = (context.SecurityToken as JwtSecurityToken)!.Payload;
            
        if(jwtService!.RefreshSession(payload, context.Request.Cookies["refresh_token"]))
            return Task.CompletedTask;
            
            
        context.Fail("Forbidden");
        return Task.CompletedTask;
    }*/
    
    
    private static void ConfigureAuthenticationAndAuthorization(WebApplicationBuilder builder)
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
        });

        builder.Services
            .AddAuthorizationBuilder()
            .AddPolicy("Authenticated", policy =>
            {
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("Admin", policy =>
            {
                policy.RequireRole("Admin");
            });
    }

}