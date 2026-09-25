using Api.Extensions;
using Contents.Application;
using Identity.Application;
using Identity.Infrastructure;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCustomSwagger();

builder.Services.AddSharedInfrastructure();

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddContentsApplication(); 


var app = builder.Build();
await Identity.Infrastructure.Persistence.IdentityDataSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
    
app.MapControllers();

app.Run();

