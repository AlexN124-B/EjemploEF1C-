using EFCore1.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EFCore1
{
    public class Program
    {
        static void Main()
        {
			try
			{
				using var inicio = new TiendaContext();
                inicio.Database.EnsureDeleted();
                inicio.Database.EnsureCreated();
                CrearBaseDeDatos(inicio);

                ListarProductos(inicio);
            }
			catch (Exception)
			{

				throw;
			} 
        }
        static void CrearBaseDeDatos(TiendaContext context)
        {
            var portátiles = new Categoria { Nombre = "Portátiles", Descripcion = "Dispositivos portátiles" };
            var perifericos = new Categoria { Nombre = "Periféricos", Descripcion = "Teclados, ratones y otros periféricos" };
            var almacenamiento = new Categoria { Nombre = "Almacenamiento", Descripcion = "Soluciones de almacenamiento" };
            var monitores = new Categoria { Nombre = "Monitores", Descripcion = "Pantallas para computadoras" };

            var lenovo = new Marca { Nombre = "Lenovo", paisOrigen = "China" };
            var logitech = new Marca { Nombre = "Logitech", paisOrigen = "Suiza" };
            var kingston = new Marca { Nombre = "Kingston", paisOrigen = "China" };
            context.Categorias.AddRange(portátiles, perifericos, almacenamiento, monitores);
            context.Marcas.AddRange(lenovo, logitech, kingston);
            context.Productos.AddRange(
                new Producto { Nombre = "ThinkPad E14 ", Precio = 800.00m, Stock = 10, Categoria = portátiles, Marca = lenovo },
                new Producto { Nombre = "IdeaPad 3", Precio = 30.00m, Stock = 50, Categoria = portátiles, Marca = lenovo },
                new Producto { Nombre = "Teclado MX Keys", Precio = 100.00m, Stock = 20, Categoria = perifericos, Marca = logitech },
                new Producto { Nombre = "Ratón MX Master 3", Precio = 200.00m, Stock = 15, Categoria = perifericos, Marca = logitech },
                new Producto { Nombre = "Ratón USB genérico", Precio = 200.00m, Stock = 15, Categoria = perifericos},
                new Producto { Nombre = "SSD NV2 1 TB", Precio = 200.00m, Stock = 15, Categoria = almacenamiento, Marca = kingston },
                new Producto { Nombre = "Pendrive 64 GB genérico", Precio = 200.00m, Stock = 15, Categoria = almacenamiento}
            );
            context.SaveChanges();
        }

        static string LeerTexto(string pregunta, bool obligatorio, int maximo)
        {
            while (true)
            {
                Console.WriteLine(pregunta);
                string texto = (Console.ReadLine() ?? "").Trim();
                if (texto.Length == 0 && obligatorio){Console.WriteLine("Este dato es obligatorio.");continue;}
                if (texto.Length > maximo){Console.WriteLine($"El texto no puede superar los {maximo} caracteres.");continue;}
                return texto;

            }
        }
        static int leerEntero(string pregunta, int minimo)
        {
            while (true)
            {
                Console.Write(pregunta);
                if (int.TryParse(Console.ReadLine(), out int numero) && numero >= minimo) return numero;
                Console.WriteLine($"Escribe un numero mayor o igual que {minimo}.");
            }
        }
        static decimal leerPrecio(string pregunta)
        {
            while (true)
            {
                Console.Write(pregunta);
                if (decimal.TryParse(Console.ReadLine(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out decimal precio) && precio > 0) return precio;
                Console.WriteLine("Escribe un numero mayor que cero.");
            }
        }
        static void guardar(TiendaContext db, string mensajeExito)
        {
            try
            {
                int filas = db.SaveChanges();
                Console.WriteLine($"{mensajeExito} {filas} filas afectadas.");

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error al guardar los cambios.");
            }
        }

        static void ListarCategorias(TiendaContext db)
        {
            var categorias = db.Categorias
                .OrderBy(c => c.Id)
                .Select(c => new { c.Id, c.Nombre, Total = c.Productos.Count()})
                .ToList();

            foreach (var categoria in categorias)
            {
                Console.WriteLine($"Id: {categoria.Id}, Nombre: {categoria.Nombre}, Total: {categoria.Total, -20}");
            }
        }

        static Categoria? ElegirCategoria(TiendaContext db)
        {
            ListarCategorias(db);
            var categoria = db.Categorias.Find(leerEntero("Elige el Id de la categoria: ", 1));
            if (categoria == null)
            {
                Console.WriteLine("No existe una categoría con ese ID.");
            }
            return categoria;
        }
        static void ListarMarcas(TiendaContext db)
        {
            var marcas = db.Marcas
                .OrderBy(m => m.Id)
                .Select(m => new { m.Id, m.Nombre, Total = m.Productos.Count() })
                .ToList();
            foreach (var marca in marcas)
            {
                Console.WriteLine($"Id: {marca.Id}, Nombre: {marca.Nombre}, Total: {marca.Total,-20}");
            }
        }
        static (bool ok, Marca? marca) ElegirMarca(TiendaContext db)
        {
            ListarMarcas(db);
            int id = leerEntero("Elige el Id de la marca: ", 0);
            if (id == 0) return (true, null);
            var marca = db.Marcas.Find(id);
            if (marca == null)
            {
                Console.WriteLine("No existe esta marca.");
                return (false, null);
            }
            return (true, marca);
        }

        static void ListarProductos(TiendaContext db)
        {
            var productos = db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .OrderBy(p => p.Id)
                .ToList();
            foreach (var producto in productos)
            {
                Console.WriteLine($"Id: {producto.Id}, Nombre: {producto.Nombre}, Precio: {producto.Precio, 8:C2}, Stock: {producto.Stock}, Categoria: {producto.Categoria.Nombre}, Marca: {producto.Marca?.Nombre ?? "Sin marca"}");
            }
        }

        static void CrearCategoria(TiendaContext db)
        {
            string nombre = LeerTexto("Nombre de la categoría: ", true, 50);
            string descripcion = LeerTexto("Descripción de la categoría: ", false, 200);
            db.Categorias.Add(new Categoria { Nombre = nombre, Descripcion = descripcion });
            guardar(db, "Categoría creada correctamente.");
        }
        static void CrearProducto(TiendaContext db)
        {
            string nombre = LeerTexto("Nombre del producto: ", true, 50);
            decimal precio = leerPrecio("Precio del producto: ");
            int stock = leerEntero("Stock del producto: ", 0);
            string referencia = LeerTexto("Referencia del producto: ", false, 200);

            var categoria = ElegirCategoria(db);
            if (categoria == null) return;
            var (ok, marca) = ElegirMarca(db);
            if (!ok) return;

            db.Productos.Add(new Producto
            {
                Nombre = nombre,
                Precio = precio,
                Stock = stock,
                Referencia = referencia == "" ? null : referencia,
                Categoria = categoria,
                Marca = marca
            });
            guardar(db, "Producto creado correctamente.");
        }
        static void CrearMarca(TiendaContext db)
        {
            string nombre = LeerTexto("Nombre de la marca: ", true, 50);
            string paisOrigen = LeerTexto("País de origen de la marca: ", false, 50);
            db.Marcas.Add(new Marca { Nombre = nombre, paisOrigen = paisOrigen == "" ? null : paisOrigen });
            guardar(db, "Marca creada correctamente.");
        }

        static void ModificarProducto(TiendaContext db)
        {
            ListarProductos(db);
            var producto = db.Productos.Find(leerEntero("Elige el Id del producto a modificar: ", 1));
            if(producto == null)
            {
                Console.WriteLine("No existe un producto con ese ID.");
                return;
            }
            string nombre = LeerTexto($"Nombre del producto ({producto.Nombre}) (Intro = no cambiar): ", true, 50);
            if (nombre != "")
            {
                producto.Nombre = nombre;
            }
            string precioStr = LeerTexto($"Precio del producto ({producto.Precio:C2}) (Intro = no cambiar): ", true, 50);
            if (precioStr != "")
            {
                producto.Precio = decimal.Parse(precioStr);
            }
            int stock = leerEntero($"Stock del producto ({producto.Stock}) (Intro = no cambiar): ", 0);
            if (stock != 0)
            {
                producto.Stock = stock;
            }
            var categoria = ElegirCategoria(db);
            if (categoria == null)
            {
                return;
            }
            producto.Categoria = categoria;
            var (ok, marca) = ElegirMarca(db);
            if (ok)
            {
                producto.Marca = marca;
            }
            guardar(db, "Producto modificado correctamente.");
        }

        static void BorrarCategoria(TiendaContext db)
        {
            ListarCategorias(db);
            var categoria = db.Categorias.Find(leerEntero("Elige el Id de la categoría a borrar: ", 1));
            if (categoria == null)
            {
                Console.WriteLine("No existe una categoría con ese ID.");
                return;
            }
            if (categoria.Productos.Count > 0)
            {
                Console.WriteLine("No se puede borrar una categoría que tiene productos asociados.");
                return;
            }
            db.Categorias.Remove(categoria);
            guardar(db, "Categoría borrada correctamente.");
        }

        static void BorrarProducto(TiendaContext db)
        {
            ListarProductos(db);
            var producto = db.Productos.Find(leerEntero("Elige el Id del producto a borrar: ", 1));
            if (producto == null)
            {
                Console.WriteLine("No existe un producto con ese ID.");
                return;
            }
            db.Productos.Remove(producto);
            guardar(db, "Producto borrado correctamente.");
        }
        static void BorrarMarca(TiendaContext db)
        {
            ListarMarcas(db);
            var marca = db.Marcas.Find(leerEntero("Elige el Id de la marca a borrar: ", 1));
            if (marca == null)
            {
                Console.WriteLine("No existe una marca con ese ID.");
                return;
            }
            if (marca.Productos.Count > 0)
            {
                Console.WriteLine("No se puede borrar una marca que tiene productos asociados.");
                return;
            }
            db.Marcas.Remove(marca);
            guardar(db, "Marca borrada correctamente.");
        }
    }
}
