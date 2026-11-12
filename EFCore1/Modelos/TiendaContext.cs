using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore1.Modelos
{
    public class TiendaContext : DbContext
    {
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            option.UseMySql(Conexion.CadenaDeConexion(), new MariaDbServerVersion(new Version(10, 11, 0))).LogTo(Console.WriteLine, new[]
                {RelationalEventId.CommandExecuted}, Microsoft.Extensions.Logging.LogLevel.Information).EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>().HasIndex(e => e.Nombre).IsUnique();
            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasPrecision(8, 2);
        }

    }
}
