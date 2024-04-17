using System.ComponentModel.DataAnnotations;

namespace _20241CYA12A_G2.Models
{

    public class Contacto
    {
        public int Id { get; set; }
        [Display(Name = "Nombre compelto")]
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [MaxLength(255, ErrorMessage = "El nombre no debe tener más de 255 caracteres")]
        public string NombreCompleto { get; set; }
        [Required(ErrorMessage = "El email es obligatorio")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El telefono es obligatorio")]
        [MinLength(10, ErrorMessage = "El teléfono debe tener 10 caracteres")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [MaxLength(4000, ErrorMessage = "El mensaje no debe tener más de 4000 caracteres")]
        public string Mensaje { get; set; }
        [Display(Name = "Leído")]
        public bool? Leido { get; set; }


    }
}
