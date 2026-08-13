using GestaodeReembolsos.Data.Mappings;
using GestaodeReembolsos.Extensions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Data;

public class GestaoDeReembolsoContext(DbContextOptions<GestaoDeReembolsoContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<ReimbursementRequest> ReimbursementRequests { get; set; }
    public DbSet<ExpenseItem> ExpenseItems { get; set; }
    public DbSet<ApprovalDecision> ApprovalDecisions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(GestaoDeReembolsoContext).Assembly
        );
        
        builder.Seed();
        
        base.OnModelCreating(builder);
    }
}