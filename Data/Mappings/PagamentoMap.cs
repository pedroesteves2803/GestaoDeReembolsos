using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class PagamentoMap : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.ReferenciaPagamento)
            .HasColumnName("ReferenciaPagamento")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.ValorPago)
            .HasColumnName("ValorPago")
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        
        builder.Property(x => x.DataPagamento)
            .HasColumnName("DataPagamento")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.Observacoes)
            .HasColumnName("Observacoes")
            .HasMaxLength(500);
        
        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.HasOne(x => x.SolicitacaoReembolso)
            .WithOne(x => x.Pagamento)
            .HasForeignKey<Pagamento>(x => x.SolicitacaoReembolsoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProcessadoPorUsuario)
            .WithMany(x => x.Pagamentos)
            .HasForeignKey(x => x.ProcessadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.ReferenciaPagamento)
            .IsUnique();
        
    }
}
