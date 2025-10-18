using Microsoft.EntityFrameworkCore;
using mvc_purple.Models;

namespace mvc_purple.Data
{
    public class JoyeriaDbContext : DbContext
    {
        public JoyeriaDbContext(DbContextOptions<JoyeriaDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(dp => dp.PedidoId);

            modelBuilder.Entity<DetallePedido>()
                .HasOne(dp => dp.Producto)
                .WithMany(p => p.DetallesPedido)
                .HasForeignKey(dp => dp.ProductoId);

            // Configuración de precisión para decimales
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>()
                .Property(dp => dp.PrecioUnitario)
                .HasPrecision(10, 2);

            // Datos iniciales (Seed)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Cliente administrador
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Email = "admin@joyeria.com",
                    Password = "admin123", // En producción debería estar hasheada
                    Direccion = "Oficina Principal",
                    EsAdmin = true,
                    FechaRegistro = DateTime.Now
                }
            );

            // Productos iniciales
            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    Id = 1,
                    Nombre = "Anillo de Oro 18k",
                    Descripcion = "Elegante anillo de oro de 18 quilates con acabado brillante",
                    Precio = 850.00m,
                    Stock = 15,
                    ImagenUrl = "/images/anillo-oro.jpg",
                    Disponible = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Collar de Plata",
                    Descripcion = "Hermoso collar de plata 925 con colgante de corazón",
                    Precio = 320.00m,
                    Stock = 25,
                    ImagenUrl = "/images/collar-plata.jpg",
                    Disponible = true,
                    FechaCreacion = DateTime.Now
                },
                new Producto
                {
                    Id = 3,
                    Nombre = "Aretes de Diamante",
                    Descripcion = "Exclusivos aretes con diamantes naturales",
                    Precio = 1200.00m,
                    Stock = 8,
                    ImagenUrl = "/images/aretes-diamante.jpg",
                    Disponible = true,
                    FechaCreacion = DateTime.Now
                }
            );
        }
    }
}