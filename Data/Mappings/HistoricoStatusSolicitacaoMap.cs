using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class HistoricoStatusSolicitacaoMap : IEntityTypeConfiguration<HistoricoStatusSolicitacao>
{
    public void Configure(EntityTypeBuilder<HistoricoStatusSolicitacao> builder)
    {
        builder.ToTable("HistoricosStatusSolicitacao");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.StatusAnterior)
            .HasColumnName("StatusAnterior")
            .HasConversion<string>();
        
        builder.Property(x => x.NovoStatus)
            .HasColumnName("NovoStatus")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Motivo)
            .HasColumnName("Motivo")
            .HasMaxLength(500);
        
        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();
        
        builder
            .HasOne(x => x.SolicitacaoReembolso)
            .WithMany(x => x.HistoricosStatusSolicitacao)
            .HasForeignKey(x => x.SolicitacaoReembolsoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.AlteradoPorUsuario)
            .WithMany(x => x.HistoricosStatusSolicitacao)
            .HasForeignKey(x => x.AlteradoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
