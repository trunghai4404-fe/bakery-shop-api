using Api.Extensions;
using Catalog.Application;
using Catalog.Infrastructure;
using Contents.Application;
using Contents.Infrastructure;
using Identity.Application;
using Identity.Infrastructure;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCustomSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSharedInfrastructure();

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddContentsInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddContentsApplication(); 
builder.Services.AddCatalogApplication();

var app = builder.Build();
await Identity.Infrastructure.Persistence.IdentityDataSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
    
app.MapControllers();

app.Run();
