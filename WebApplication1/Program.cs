using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("postgresdb")
         ?? throw new InvalidOperationException("Connection string 'postgresdb' not found.");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("ui", p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ui");

app.UseAuthorization();

app.MapControllers();

if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
