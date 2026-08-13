using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class ApprovalDecisionMap : IEntityTypeConfiguration<ApprovalDecision>
{
    public void Configure(EntityTypeBuilder<ApprovalDecision> builder)
    {
        builder.ToTable("ApprovalDecisions");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.DecisionLevel)
            .HasColumnName("DecisionLevel")
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(x => x.Decision)
            .HasColumnName("Decision")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("Comment")
            .HasMaxLength(500);
        
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();
        
        builder.HasOne(x => x.ReimbursementRequest)
            .WithMany(x => x.ApprovalDecisions)
            .HasForeignKey(x => x.ReimbursementRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.DecidedByUser)
            .WithMany(x => x.ApprovalDecisions)
            .HasForeignKey(x => x.DecidedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}