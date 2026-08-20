using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class DecisaoAprovacaoMap : IEntityTypeConfiguration<DecisaoAprovacao>
{
    public void Configure(EntityTypeBuilder<DecisaoAprovacao> builder)
    {
        builder.ToTable("DecisoesAprovacao");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.NivelDecisao)
            .HasColumnName("NivelDecisao")
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(x => x.Decisao)
            .HasColumnName("Decisao")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Comentario)
            .HasColumnName("Comentario")
            .HasMaxLength(500);
        
        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();
        
        builder.HasOne(x => x.SolicitacaoReembolso)
            .WithMany(x => x.DecisoesAprovacao)
            .HasForeignKey(x => x.SolicitacaoReembolsoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.DecidedByUser)
            .WithMany(x => x.DecisoesAprovacao)
            .HasForeignKey(x => x.DecididaPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}