namespace _20241CYA12A_G2.Models
{

    public class Carrito
    {
        public int Id { get; set; }
         public bool Procesado { get; set; }
        public bool Cancelado { get; set; }

        //Relaciones
        public int ClenteId { get; set; }
        public Cliente? Cliente { get; set; }
        public virtual Pedido? Pedido { get; set; }
        public ICollection<CarritoItem>? CarritoItems {  get; set; }



    }
}
