using System;

namespace _20241CYA12A_G2.Models
{

    public class Reserva
    {
        public int ClienteId { get; set; }
       // public Cliente Cliente { get; set; }
        public string Local { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Confirmada { get; set; }


    }
}
