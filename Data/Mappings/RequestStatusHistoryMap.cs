using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class RequestStatusHistoryMap : IEntityTypeConfiguration<RequestStatusHistory>
{
    public void Configure(EntityTypeBuilder<RequestStatusHistory> builder)
    {
        builder.ToTable("RequestStatusHistories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PreviousStatus)
            .HasColumnName("PreviousStatus")
            .HasConversion<string>();
        
        builder.Property(x => x.NewStatus)
            .HasColumnName("NewStatus")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasColumnName("Reason")
            .HasMaxLength(500);
        
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();
        
        builder
            .HasOne(x => x.ReimbursementRequest)
            .WithMany(x => x.RequestStatusHistories)
            .HasForeignKey(x => x.ReimbursementRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.ChangedByUser)
            .WithMany(x => x.RequestStatusHistories)
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}