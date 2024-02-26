using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using viki_01.Contexts;
using viki_01.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<WikiHostingSqlServerContext>(options =>
{
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    // TODO configure CORS for prod environment
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("http://localhost:4200", "https://*.gitlab.io")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddScoped<IAuthService, SqlDbAuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => AuthOptions.ConfigureJwtBearer(options));

builder.Services.AddSingleton<IUserIdProvider, SignalrEmailBasedUserIdProvider>();

builder.Services.AddControllers();
builder.Services.AddHostedService<TokenCleanerService>();

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();
