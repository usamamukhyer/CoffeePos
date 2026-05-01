using DBCafeteria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Infrastructure.Data;

public sealed class CafeDbContext(DbContextOptions<CafeDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Clasificacion> Clasificaciones => Set<Clasificacion>();
    public DbSet<SubClasificacion> SubClasificaciones => Set<SubClasificacion>();
    public DbSet<Leche> Leches => Set<Leche>();
    public DbSet<TipoGrano> TiposGranos => Set<TipoGrano>();
    public DbSet<Topping> Toppings => Set<Topping>();
    public DbSet<Vaso> Vasos => Set<Vaso>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoProducto> PedidosProductos => Set<PedidoProducto>();
    public DbSet<PedidoProductoTopping> PedidosProductosToppings => Set<PedidoProductoTopping>();
    public DbSet<PedidoPago> PedidosPagos => Set<PedidoPago>();
    public DbSet<Regalo> Regalos => Set<Regalo>();
    public DbSet<Sucursal> Sucursales => Set<Sucursal>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<SubClasificacionLeche> SubClasificacionesLeches => Set<SubClasificacionLeche>();
    public DbSet<SubClasificacionTipoGrano> SubClasificacionesTiposGranos => Set<SubClasificacionTipoGrano>();
    public DbSet<SubClasificacionTopping> SubClasificacionesToppings => Set<SubClasificacionTopping>();
    public DbSet<SubClasificacionVaso> SubClasificacionesVasos => Set<SubClasificacionVaso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.Property(x => x.Telefono).HasColumnName("Telefono");
            entity.Property(x => x.Nombre).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Email).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.FechaRegistro).HasDefaultValueSql("getdate()");
            entity.Property(x => x.Activo).HasDefaultValue(true);
            entity.Property(x => x.EsInvitado).HasDefaultValue(false);
        });

        modelBuilder.Entity<Clasificacion>(entity =>
        {
            entity.ToTable("Clasificaciones");
            entity.Property(x => x.Descripcion).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Clave).HasMaxLength(255).IsUnicode(false);
            entity.HasMany(x => x.SubClasificaciones).WithOne(x => x.Clasificacion).HasForeignKey(x => x.IdClasificacion);
        });

        modelBuilder.Entity<SubClasificacion>(entity =>
        {
            entity.ToTable("SubClasificaciones");
            entity.Property(x => x.Clave).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Descripcion).HasMaxLength(255).IsUnicode(false);
        });

        modelBuilder.Entity<SubClasificacionLeche>(entity =>
        {
            entity.ToTable("SubClasificacionesLeches");
            entity.HasOne(x => x.Leche).WithMany().HasForeignKey(x => x.IdLeche);
            entity.HasOne(x => x.SubClasificacion).WithMany().HasForeignKey(x => x.IdSubClasificacion);
        });

        modelBuilder.Entity<SubClasificacionTipoGrano>(entity =>
        {
            entity.ToTable("SubClasificacionesTiposGranos");
            entity.HasOne(x => x.TipoGrano).WithMany().HasForeignKey(x => x.IdTipoGrano);
            entity.HasOne(x => x.SubClasificacion).WithMany().HasForeignKey(x => x.IdSubClasificacion);
        });

        modelBuilder.Entity<SubClasificacionTopping>(entity =>
        {
            entity.ToTable("SubClasificacionesToppings");
            entity.HasOne(x => x.Topping).WithMany().HasForeignKey(x => x.IdTopping);
            entity.HasOne(x => x.SubClasificacion).WithMany().HasForeignKey(x => x.IdSubClasificacion);
        });

        modelBuilder.Entity<SubClasificacionVaso>(entity =>
        {
            entity.ToTable("SubClasificacionesVasos");
            entity.HasOne(x => x.Vaso).WithMany().HasForeignKey(x => x.IdVaso);
            entity.HasOne(x => x.SubClasificacion).WithMany().HasForeignKey(x => x.IdSubClasificacion);
        });

        modelBuilder.Entity<Leche>(entity =>
        {
            entity.ToTable("Leches");
            entity.Property(x => x.Clave).HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.Descripcion).HasMaxLength(250).IsUnicode(false);
        });

        modelBuilder.Entity<TipoGrano>(entity =>
        {
            entity.ToTable("TiposGranos");
            entity.Property(x => x.Clave).HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.Descripcion).HasMaxLength(150).IsUnicode(false);
        });

        modelBuilder.Entity<Topping>(entity =>
        {
            entity.ToTable("Toppings");
            entity.Property(x => x.Clave).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Descripcion).HasMaxLength(255).IsUnicode(false);
        });

        modelBuilder.Entity<Vaso>(entity =>
        {
            entity.ToTable("Vasos");
            entity.Property(x => x.Clave).HasMaxLength(20).IsUnicode(false);
            entity.Property(x => x.Descripcion).HasMaxLength(250).IsUnicode(false);
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.ToTable("Sucursales");
            entity.Property(x => x.Nombre).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Direccion).HasMaxLength(500).IsUnicode(false);
            entity.Property(x => x.Telefono).HasMaxLength(50).IsUnicode(false);
            entity.Property(x => x.Activo).HasDefaultValue(true);
            entity.Property(x => x.FechaCreacion).HasDefaultValueSql("getdate()");
            entity.HasMany(x => x.Pedidos).WithOne(x => x.Sucursal).HasForeignKey(x => x.IdSucursal);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedidos");
            entity.Property(x => x.TipoPedido).HasMaxLength(50).IsUnicode(false).HasDefaultValue("ForMe");
            entity.Property(x => x.TipoPickup).HasMaxLength(50).IsUnicode(false).HasDefaultValue("Now");
            entity.Property(x => x.Ubicacion).HasMaxLength(255).IsUnicode(false);
            entity.HasOne(x => x.Cliente).WithMany(x => x.Pedidos).HasForeignKey(x => x.IdCliente);
            entity.HasOne(x => x.Sucursal).WithMany(x => x.Pedidos).HasForeignKey(x => x.IdSucursal);
            entity.HasMany(x => x.Productos).WithOne(x => x.Pedido).HasForeignKey(x => x.IdPedido);
            entity.HasMany(x => x.Pagos).WithOne(x => x.Pedido).HasForeignKey(x => x.IdPedido);
        });

        modelBuilder.Entity<PedidoProducto>(entity =>
        {
            entity.ToTable("PedidosProductos");
            entity.HasMany(x => x.Toppings).WithOne(x => x.PedidoProducto).HasForeignKey(x => x.IdPedidoProducto);
        });

        modelBuilder.Entity<PedidoProductoTopping>().ToTable("PedidosProductosToppings");

        modelBuilder.Entity<PedidoPago>(entity =>
        {
            entity.ToTable("PedidosPagos");
            entity.Property(x => x.FormaPago).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Moneda).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Estado).HasMaxLength(50).IsUnicode(false).HasDefaultValue("Pending");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.Property(x => x.UserName).HasColumnName("Usuario").HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Nombre).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.Tipo).HasDefaultValue(0);
        });

        modelBuilder.Entity<Regalo>(entity =>
        {
            entity.ToTable("Regalos");
            entity.Property(x => x.NombreReceptor).HasMaxLength(255).IsUnicode(false);
            entity.Property(x => x.TelefonoReceptor).HasMaxLength(50).IsUnicode(false);
            entity.Property(x => x.Mensaje).HasMaxLength(500).IsUnicode(false);
            entity.Property(x => x.CodigoRegalo).HasMaxLength(100).IsUnicode(false);
            entity.Property(x => x.Estado).HasMaxLength(50).IsUnicode(false).HasDefaultValue("Pending");
            entity.Property(x => x.FechaCreacion).HasDefaultValueSql("getdate()");
            entity.HasOne(x => x.Pedido).WithOne(x => x.Regalo).HasForeignKey<Regalo>(x => x.IdPedido);
        });
    }
}
