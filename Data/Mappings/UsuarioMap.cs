using GestaodeReembolsos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaodeReembolsos.Data.Mappings;
 
public class UsuarioMap : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        
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
            .HasColumnName("PerfilUsuario")
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
            .HasOne(x => x.Gestor)
            .WithMany(x => x.Subordinados)
            .HasForeignKey(x => x.GestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Departamento)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(x => x.Email, "IX_Usuario_Email")
            .IsUnique();    
    }
}
