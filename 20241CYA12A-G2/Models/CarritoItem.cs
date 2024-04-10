namespace _20241CYA12A_G2.Models
{

    public class CarritoItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int CarritoId { get; set; }

        public double PrecioUnitarioConDescuento { get; set; }
        public int Cantidad { get; set; }


    }
}