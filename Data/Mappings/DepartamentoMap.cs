using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class DepartamentoMap: IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.ToTable("Departments");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.CodigoCentroCusto)
            .HasColumnName("CodigoCentroCusto")
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(x => x.Ativo)
            .HasColumnName("Ativo")
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();
        
        builder
            .HasIndex(x => x.Name, "IX_Department_Name")
            .IsUnique();    
        
        builder
            .HasIndex(x => x.CodigoCentroCusto, "IX_Department_CostCenterCode")
            .IsUnique();    
    }
}