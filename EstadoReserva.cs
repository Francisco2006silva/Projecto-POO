using System;

namespace GestaoEspacosDesportivos
{
    public class Reserva
    {
        private int id;
        private DateTime data;
        private TimeSpan horaInicio;
        private TimeSpan horaFim;
        private EstadoReserva estado;

        // Acesso de leitura (necessário para EspacoDesportivo.VerificarDisponibilidade)
        public int Id => id;
        public DateTime Data => data;
        public TimeSpan HoraInicio => horaInicio;
        public TimeSpan HoraFim => horaFim;
        public EstadoReserva Estado => estado;

        public Reserva(int id, DateTime data, TimeSpan horaInicio, TimeSpan horaFim)
        {
            this.id = id;
            this.data = data;
            this.horaInicio = horaInicio;
            this.horaFim = horaFim;
            this.estado = EstadoReserva.Pendente;
        }

        public void Confirmar()
        {
            estado = EstadoReserva.Confirmada;
        }

        public void Cancelar()
        {
            estado = EstadoReserva.Cancelada;
        }

        public void Concluir()
        {
            estado = EstadoReserva.Concluida;
        }

        public void Expirar()
        {
            estado = EstadoReserva.Expirada;
        }
    }
}