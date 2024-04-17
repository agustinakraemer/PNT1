using System.ComponentModel.DataAnnotations;

namespace _20241CYA12A_G2.Models
{

    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Categoria es obligatorio")]
        [MaxLength(100, ErrorMessage = "Categoria no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }
        [Display(Name = "Descripción")]
        [MaxLength(4000, ErrorMessage = "Categoria no puede tener más de 4000 caracteres")]
        public string? Descripcion { get; set; }

        //Relaciones
        public ICollection<Producto>? Productos { get; set; }


    }
}
