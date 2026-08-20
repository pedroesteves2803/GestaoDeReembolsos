using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;
 
public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.NomeCompleto)
            .HasColumnName("NomeCompleto")
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(x => x.Email)
            .HasColumnName("Email")
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(x => x.HashSenha)
            .HasColumnName("HashSenha")
            .IsRequired();
        
        builder.Property(x => x.PerfilUsuario)
            .HasColumnName("Role")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Ativo)
            .HasColumnName("Ativo")
            .HasConversion<bool>()
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(x => x.CriadaEmUtc)
            .HasColumnName("CriadaEmUtc")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("(SYSUTCDATETIME())")
            .IsRequired();

        builder.Property(x => x.AtualizadaEmUtc)
            .HasColumnName("AtualizadaEmUtc")
            .HasColumnType("datetime2");
        
        builder
            .HasOne(x => x.Manager)
            .WithMany(x => x.Subordinados)
            .HasForeignKey(x => x.GestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Departamento)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(x => x.Email, "IX_User_Email")
            .IsUnique();    
    }
}