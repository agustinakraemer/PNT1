namespace _20241CYA12A_G2.Models
{

    public class Reclamo
    {
        public int Id { get; set; }
        public string NombreConmpleto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string DetalleReclamo { get; set; }

        //Relaciones
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

    }
}