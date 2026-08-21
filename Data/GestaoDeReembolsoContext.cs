using GestaodeReembolsos.Data.Mappings;
using GestaodeReembolsos.Extensions;
using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Data;

public class GestaoDeReembolsoContext(DbContextOptions<GestaoDeReembolsoContext> options) 
    : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<CategoriaDespesa> CategoriasDespesa { get; set; }
    public DbSet<SolicitacaoReembolso> SolicitacoesReembolso { get; set; }
    public DbSet<ItemDespesa> ItensDespesa { get; set; }
    public DbSet<DecisaoAprovacao> DecisoesAprovacao { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
    public DbSet<HistoricoStatusSolicitacao> HistoricosStatusSolicitacao { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(GestaoDeReembolsoContext).Assembly
        );

        
        builder.Seed();
        
        base.OnModelCreating(builder);
    }
}