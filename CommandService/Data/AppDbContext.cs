using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;
public class AppDbContext:DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }

    public DbSet<Platform> Platforms { get; set; }=null!;
    public DbSet<Command> Commands { get; set; }=null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>(p=>{
            p.HasMany(p=>p.Commands)
            .WithOne(c=>c.Platform)
            .HasForeignKey(c=>c.PlatformId);
        });

        modelBuilder.Entity<Command>(c =>{
            c.HasOne(c=>c.Platform)
            .WithMany(p=>p.Commands)
            .HasForeignKey(p=>p.PlatformId);
        });
    }
}