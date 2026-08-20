using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class ItemDespesaMap : IEntityTypeConfiguration<ItemDespesa>
{
    public void Configure(EntityTypeBuilder<ItemDespesa> builder)
    {
        builder.ToTable("ExpenseItems");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.DataDespesa)
            .HasColumnName("DataDespesa")
            .HasColumnType("date")
            .IsRequired();
        
        builder.Property(x => x.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(300)
            .IsRequired();
        
        builder.Property(x => x.Valor)
            .HasColumnName("Valor")
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        
        builder.Property(x => x.NomeEstabelecimento)
            .HasColumnName("NomeEstabelecimento")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.NomeArquivoComprovante)
            .HasColumnName("NomeArquivoComprovante")
            .HasMaxLength(255);
        
        builder.Property(x => x.ChaveArmazenamentoComprovante)
            .HasColumnName("ChaveArmazenamentoComprovante")
            .HasMaxLength(500);
        
        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.Property(x => x.AtualizadaEmUtc)
            .HasColumnName("AtualizadaEmUtc")
            .HasColumnType("datetime2");

        builder.HasOne(x => x.SolicitacaoReembolso)
            .WithMany(x => x.ExpenseItems)
            .HasForeignKey(x => x.SolicitacaoReembolsoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.CategoriaDespesa)
            .WithMany(x => x.ExpenseItems)
            .HasForeignKey(x => x.CategoriaDespesaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}