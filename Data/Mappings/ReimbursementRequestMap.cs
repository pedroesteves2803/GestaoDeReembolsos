using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;

public class ReimbursementRequestMap : IEntityTypeConfiguration<ReimbursementRequest>
{
    public void Configure(EntityTypeBuilder<ReimbursementRequest> builder)
    {
        builder.ToTable("ReimbursementRequests");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.RequestNumber)
            .HasColumnName("RequestNumber")
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(x => x.ReferenceMonth)
            .HasColumnName("ReferenceMonth")
            .HasColumnType("date")
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(x => x.TotalAmount)
            .HasColumnName("TotalAmount")
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        
        builder.Property(x => x.SubmittedAtUtc)
            .HasColumnName("SubmittedAtUtc")
            .HasColumnType("datetime2");
        
        builder.Property(x => x.ManagerDecisionAtUtc)
            .HasColumnName("ManagerDecisionAtUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.FinanceDecisionAtUtc)
            .HasColumnName("FinanceDecisionAtUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.PaidAtUtc)
            .HasColumnName("PaidAtUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("UpdatedAtUtc")
            .HasColumnType("datetime2");

        builder.Property(x => x.Version)
            .HasColumnName("Version")
            .IsRowVersion();
        
        builder
            .HasOne(x => x.Employee)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(x => x.Department)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.RequestNumber, "IX_ReimbursementRequest_RequestNumber")
            .IsUnique();
    }
}