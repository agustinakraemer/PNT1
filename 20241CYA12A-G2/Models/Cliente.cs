namespace _20241CYA12A_G2.Models
{

    public class Cliente : Usuario
    {
        public int Id { get; set; }
        public bool Procesado { get; set; }
        public bool Cancelado { get; set; }

        public int ClenteId { get; set; }


    }
}
