namespace _20241CYA12A_G2.Models
{
    public class DetallePedidoViewModel
    {
        public string Cliente { get; set; }
        public List<string> Productos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GastoEnvio { get; set; }
        public decimal Total { get; set; }
    }
}
