using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class AppDbContext : DbContext
{
    public DbSet<Todo> Todos => Set<Todo>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>(b =>
        {
            b.ToTable("todos");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.IsDone);
            b.HasIndex(x => x.IsDone);
        });
    }
}
