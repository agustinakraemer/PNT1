namespace _20241CYA12A_G2.Models
{

    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }

        //Relaciones
        public ICollection<Producto>? Productos { get; set; }
       


    }
}
