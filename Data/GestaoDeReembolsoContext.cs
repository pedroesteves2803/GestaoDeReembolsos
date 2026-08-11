using GestaodeReembolsos.Extensions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Data;

public class GestaoDeReembolsoContext(DbContextOptions<GestaoDeReembolsoContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(GestaoDeReembolsoContext).Assembly
        );
        
        builder.Seed();
        
        base.OnModelCreating(builder);
    }
}