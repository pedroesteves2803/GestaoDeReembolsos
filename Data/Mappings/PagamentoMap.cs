using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class PaymentMap : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Payments");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.ReferenciaPagamento)
            .HasColumnName("ReferenciaPagamento")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.PaidAmount)
            .HasColumnName("PaidAmount")
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        
        builder.Property(x => x.PaymentDate)
            .HasColumnName("PaymentDate")
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

        builder.HasOne(x => x.ProcessedByUser)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.ProcessadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.ReferenciaPagamento)
            .IsUnique();
        
    }
}