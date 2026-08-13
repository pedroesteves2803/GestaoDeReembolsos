using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class ExpenseItemMap : IEntityTypeConfiguration<ExpenseItem>
{
    public void Configure(EntityTypeBuilder<ExpenseItem> builder)
    {
        builder.ToTable("ExpenseItems");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.ExpenseDate)
            .HasColumnName("ExpenseDate")
            .HasColumnType("date")
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .HasMaxLength(300)
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .HasColumnName("Amount")
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        
        builder.Property(x => x.MerchantName)
            .HasColumnName("MerchantName")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ReceiptFileName)
            .HasColumnName("ReceiptFileName")
            .HasMaxLength(255);
        
        builder.Property(x => x.ReceiptStorageKey)
            .HasColumnName("ReceiptStorageKey")
            .HasMaxLength(500);
        
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("UpdatedAtUtc")
            .HasColumnType("datetime2");

        builder.HasOne(x => x.ReimbursementRequest)
            .WithMany(x => x.ExpenseItems)
            .HasForeignKey(x => x.ReimbursementRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.ExpenseCategory)
            .WithMany(x => x.ExpenseItems)
            .HasForeignKey(x => x.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}