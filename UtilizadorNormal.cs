using System;
using System.Collections.Generic;

namespace GestaoEspacosDesportivos
{
    public class UtilizadorNormal : Utilizador
    {
        private readonly List<Reserva> reservas = new List<Reserva>();

        public UtilizadorNormal(int id, string nome, string email, string password)
            : base(id, nome, email, password, "Normal")
        {
        }

        // Devolve null se o espaço não estiver disponível nesse horário
        public Reserva? CriarReserva(EspacoDesportivo espaco, DateTime data, TimeSpan inicio, TimeSpan fim)
        {
            if (!espaco.VerificarDisponibilidade(data, inicio, fim))
                return null;

            Reserva nova = new Reserva(this, espaco, data, inicio, fim);
            reservas.Add(nova);
            espaco.AdicionarReserva(nova);
            return nova;
        }

        public List<Reserva> ConsultarReservas()
        {
            return new List<Reserva>(reservas);
        }

        public void CancelarReserva(Reserva reserva)
        {
            if (!reservas.Contains(reserva))
            {
                Console.WriteLine("Essa reserva não pertence a este utilizador.");
                return;
            }
            reserva.Cancelar();
        }
    }
}
