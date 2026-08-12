using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class ExpenseCategoryMap: IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(80)
            .IsRequired();
        
        builder.Property(x => x.MonthlyLimit)
            .HasColumnName("MonthlyLimit")
            .HasColumnType("decimal(12,2)");
        
        builder.Property(x => x.RequiresReceipt)
            .HasColumnName("RequiresReceipt")
            .HasColumnType("bit")
            .HasDefaultValue(true);

        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true);
        
        builder
            .HasIndex(x => x.Name, "IX_ExpenseCategory_Name")
            .IsUnique();   
    }
}