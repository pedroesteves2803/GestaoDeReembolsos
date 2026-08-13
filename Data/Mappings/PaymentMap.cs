using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class PaymentMap : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.PaymentReference)
            .HasColumnName("PaymentReference")
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

        builder.Property(x => x.Notes)
            .HasColumnName("Notes")
            .HasMaxLength(500);
        
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.HasOne(x => x.ReimbursementRequest)
            .WithOne(x => x.Payment)
            .HasForeignKey<Payment>(x => x.ReimbursementRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProcessedByUser)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.ProcessedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.PaymentReference)
            .IsUnique();
        
    }
}