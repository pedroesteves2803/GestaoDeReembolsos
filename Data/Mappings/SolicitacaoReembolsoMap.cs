using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class ReimbursementRequestMap : IEntityTypeConfiguration<SolicitacaoReembolso>
{
    public void Configure(EntityTypeBuilder<SolicitacaoReembolso> builder)
    {
        builder.ToTable("ReimbursementRequests");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.NumeroSolicitacao)
            .HasColumnName("NumeroSolicitacao")
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(x => x.MesReferencia)
            .HasColumnName("MesReferencia")
            .HasColumnType("date")
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(x => x.ValorTotal)
            .HasColumnName("ValorTotal")
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        
        builder.Property(x => x.EnviadaEmUtc)
            .HasColumnName("EnviadaEmUtc")
            .HasColumnType("datetime2");
        
        builder.Property(x => x.DecididaPeloGestorEmUtc)
            .HasColumnName("DecididaPeloGestorEmUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.DecididaPeloFinanceiroEmUtc)
            .HasColumnName("DecididaPeloFinanceiroEmUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.PagaEmUtc)
            .HasColumnName("PagaEmUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.Property(x => x.AtualizadaEmUtc)
            .HasColumnName("AtualizadaEmUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.Version)
            .HasColumnName("Version")
            .IsRowVersion();
        
        builder
            .HasOne(x => x.Employee)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.ColaboradorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Departamento)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.NumeroSolicitacao, "IX_ReimbursementRequest_RequestNumber")
            .IsUnique();
    }
}