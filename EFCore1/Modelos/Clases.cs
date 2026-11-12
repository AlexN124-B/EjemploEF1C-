using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore1.Modelos
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Nombre { get; set; }
        [MaxLength(200)]
        public string? Descripcion { get; set; }
        public List<Producto> Productos { get; set; } = new();
    }

    public class Marca
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(50)]
        public string? paisOrigen { get; set; }
        public List<Producto> Productos { get; set; } = new();
    }
    public class Producto
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        [MaxLength(20)]
        public string? Referencia { get; set; }

        //Relaciones obligatorias
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
        //Relaciones opcionales
        public int? MarcaId { get; set; }
        public Marca? Marca { get; set; }
    }
}
