using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class CategoriaDespesaMap: IEntityTypeConfiguration<CategoriaDespesa>
{
    public void Configure(EntityTypeBuilder<CategoriaDespesa> builder)
    {
        builder.ToTable("CategoriasDespesa");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(80)
            .IsRequired();
        
        builder.Property(x => x.LimiteMensal)
            .HasColumnName("LimiteMensal")
            .HasColumnType("decimal(12,2)");
        
        builder.Property(x => x.ExigeComprovante)
            .HasColumnName("ExigeComprovante")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(x => x.Ativo)
            .HasColumnName("Ativo")
            .HasColumnType("bit")
            .HasDefaultValue(true);
        
        builder
            .HasIndex(x => x.Nome, "IX_CategoriaDespesa_Nome")
            .IsUnique();   
    }
}
