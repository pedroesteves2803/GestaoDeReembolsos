using GestaodeReembolsos.Data.Mappings;
using GestaodeReembolsos.Extensions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Data;

public class GestaoDeReembolsoContext(DbContextOptions<GestaoDeReembolsoContext> options) 
    : DbContext(options)
{
    public DbSet<Usuario> Users { get; set; }
    public DbSet<Departamento> Departments { get; set; }
    public DbSet<CategoriaDespesa> ExpenseCategories { get; set; }
    public DbSet<SolicitacaoReembolso> ReimbursementRequests { get; set; }
    public DbSet<ItemDespesa> ExpenseItems { get; set; }
    public DbSet<DecisaoAprovacao> ApprovalDecisions { get; set; }
    public DbSet<Pagamento> Payments { get; set; }
    public DbSet<HistoricoStatusSolicitacao> RequestStatusHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(GestaoDeReembolsoContext).Assembly
        );
        
        builder.Seed();
        
        base.OnModelCreating(builder);
    }
}