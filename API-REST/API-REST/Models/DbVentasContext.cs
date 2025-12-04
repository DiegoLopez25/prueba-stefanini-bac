using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API_REST.Models;

public partial class DbVentasContext : DbContext
{
    public DbVentasContext()
    {
    }

    public DbVentasContext(DbContextOptions<DbVentasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DetalleVenta> DetalleVentas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}
       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.Idde).HasName("PK__detalle___9DB7AA8494B471D9");

            entity.ToTable("detalle_ventas");

            entity.Property(e => e.Idde).HasColumnName("idde");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Idpro).HasColumnName("idpro");
            entity.Property(e => e.Idventa).HasColumnName("idventa");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("iva");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdproNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.Idpro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__detalle_v__idpro__59FA5E80");

            entity.HasOne(d => d.IdventaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.Idventa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__detalle_v__idven__59063A47");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Idpro).HasName("PK__producto__04B64501D81D4846");

            entity.ToTable("productos");

            entity.HasIndex(e => e.Codigo, "UQ__producto__40F9A2068CF83D58").IsUnique();

            entity.Property(e => e.Idpro).HasColumnName("idpro");
            entity.Property(e => e.Codigo)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Producto1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("producto");
        });

        // modelBuilder.Entity<Producto>()
        //.HasQueryFilter(p => p.DeletedAt == null);

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK__tipo_usu__CF901089649E22EE");

            entity.ToTable("tipo_usuario");

            entity.HasIndex(e => e.Nombre, "UQ__tipo_usu__72AFBCC6E5914033").IsUnique();

            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__4E3E04AD250A451D");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Usuario1, "UQ__usuarios__9AFF8FC64D51E870").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .HasColumnName("password");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(100)
                .HasColumnName("usuario");

            entity.HasOne(d => d.IdTipoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__usuarios__id_tip__5165187F");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Idventa).HasName("PK__ventas__F82D1AFB0E319A0F");

            entity.ToTable("ventas");

            entity.Property(e => e.Idventa).HasColumnName("idventa");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Idvendedor).HasColumnName("idvendedor");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdvendedorNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.Idvendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ventas__idvended__5535A963");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
