using FlatRenting.Data;
using FlatRenting.Data.Repositories;
using FlatRenting.Interfaces;
using FlatRenting.Middleware;
using FlatRenting.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Text;

try {
    var builder = WebApplication.CreateBuilder(args);

    var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
           .Build();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

    Log.Logger.Debug("Using: " + $"appsettings.{builder.Environment.EnvironmentName}.json");

    // Add services to the container.
    builder.Services.AddSingleton(Log.Logger);
    builder.Services.AddDbContext<FlatRentingContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IAnnoucementRepository, AnnoucementRepository>();
    builder.Services.AddTransient<ICommentRepository, CommentRepository>();
    builder.Services.AddTransient<IActivationRepository, ActivationRepository>();
    builder.Services.AddSingleton<IEmailService, EmailService>();
    builder.Services.AddSingleton<IPhotoService, PhotoService>();
    builder.Services.AddSingleton<ICryptoService, CryptoService>();
    builder.Services.AddSingleton<IRandomService, RandomService>();

    builder.Services.AddAuthentication(opt => {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
        };
    });
    builder.Services.AddAuthorization();

    var multiplexer = ConnectionMultiplexer.Connect(builder.Configuration["Cache:Address"]);
    builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

    builder.Services.AddCors(options => {
        options.AddDefaultPolicy(
            policy => policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().WithExposedHeaders("Authorization"));
    });

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Logging.ClearProviders();
    builder.Host.UseSerilog();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope()) {
        var db = scope.ServiceProvider.GetRequiredService<FlatRentingContext>();
        db.Database.Migrate();
    }

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<JwtMiddleware>();
    app.MapControllers();

    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}
