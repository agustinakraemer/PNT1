namespace _20241CYA12A_G2.Models
{

    public class Descuento
    {
        public int Id { get; set; }
        public int Dia { get; set; }
        public int Porcentaje { get; set; }
        public int DescuentoMaximo { get; set; }
        public bool Activo { get; set; }

        //Relaciones
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }

    }
}