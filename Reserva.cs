using System;

namespace GestaoEspacosDesportivos
{
    public class Reserva
    {
        private static int proximoId = 1;

        public int Id { get; private set; }
        public DateTime Data { get; private set; }
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFim { get; private set; }
        public EstadoReserva Estado { get; private set; }

        // Associações do diagrama: "efetua" (UtilizadorNormal) e "recebe" (EspacoDesportivo)
        public UtilizadorNormal Utilizador { get; private set; }
        public EspacoDesportivo Espaco { get; private set; }

        public bool EstaAtiva => Estado == EstadoReserva.Pendente || Estado == EstadoReserva.Confirmada;

        public Reserva(UtilizadorNormal utilizador, EspacoDesportivo espaco,
                       DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            Id = proximoId++;
            Utilizador = utilizador;
            Espaco = espaco;
            Data = data.Date;
            HoraInicio = horaInicio;
            HoraFim = horaFim;
            Estado = EstadoReserva.Pendente;
        }

        public void Confirmar()
        {
            if (Estado == EstadoReserva.Pendente)
                Estado = EstadoReserva.Confirmada;
            else
                Console.WriteLine($"Só é possível confirmar reservas pendentes (estado atual: {Estado}).");
        }

        public void Cancelar()
        {
            if (EstaAtiva)
                Estado = EstadoReserva.Cancelada;
            else
                Console.WriteLine($"Não é possível cancelar uma reserva {Estado}.");
        }

        public void Concluir()
        {
            if (Estado == EstadoReserva.Confirmada)
                Estado = EstadoReserva.Concluida;
            else
                Console.WriteLine($"Só é possível concluir reservas confirmadas (estado atual: {Estado}).");
        }

        public void Expirar()
        {
            if (Estado == EstadoReserva.Pendente)
                Estado = EstadoReserva.Expirada;
            else
                Console.WriteLine($"Só é possível expirar reservas pendentes (estado atual: {Estado}).");
        }

        public override string ToString()
        {
            string inicio = HoraInicio.ToString(@"hh\:mm");
            string fim = HoraFim.ToString(@"hh\:mm");
            return $"#{Id} | {Espaco.Nome} | {Data:yyyy-MM-dd} {inicio}-{fim} | {Utilizador.Nome} | {Estado}";
        }
    }
}
