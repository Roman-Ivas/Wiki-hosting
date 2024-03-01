using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using viki_01.Authorization;
using viki_01.Contexts;
using viki_01.Extensions;
using viki_01.Services;
using viki_01.Utils;

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

builder.Services.AddTransient<ITopicRepository, DatabaseTopicRepository>();
builder.Services.AddTransient<IPageRepository, DatabasePageRepository>();
builder.Services.AddTransient<IContributorRepository, DatabaseContributorRepository>();
builder.Services.AddScoped<IAuthService, SqlDbAuthService>();
builder.Services.AddKeyedSingleton(ServiceKeys.LoggerSerializerOptions,
    new JsonSerializerOptions
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => AuthOptions.ConfigureJwtBearer(options));

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy(name: "AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy(name: "ModerationOnly", policy => policy.RequireRole("Admin", "Moderator"));
        options.AddPolicy(name: "PageUpsert", policy => policy.Requirements.Add(new OperationAuthorizationRequirement { Name = "PageUpsert"}));
    });

builder.Services.AddSingleton<IUserIdProvider, SignalrEmailBasedUserIdProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PageAuthorizationHandler>();

builder.Services.AddControllers();
builder.Services.AddHostedService<TokenCleanerService>();

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMappers(mapperBuilder =>
{
    mapperBuilder.AddMapper<TopicToTopicDtoMapper>()
        .AddMapper<TopicToTopicUpsertDtoMapper>()
        .AddMapper<PageToPageDtoMapper>()
        .AddMapper<PageToPageUpsertDtoMapper>();
});

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
