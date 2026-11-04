using System;
using System.Collections.Generic;

namespace GestaoEspacosDesportivos
{
    public class EspacoDesportivo
    {
        private readonly List<Equipamento> equipamentos = new List<Equipamento>();
        private readonly List<Reserva> reservas = new List<Reserva>();

        public int Id { get; private set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Localizacao { get; set; }
        public int CapacidadeMaxima { get; set; }
        public bool Disponivel { get; set; }

        // "possui" (agregação) e "recebe"
        public IReadOnlyList<Equipamento> Equipamentos => equipamentos;
        public IReadOnlyList<Reserva> Reservas => reservas;

        public EspacoDesportivo(int id, string nome, string tipo, string localizacao, int capacidadeMaxima)
        {
            Id = id;
            Nome = nome;
            Tipo = tipo;
            Localizacao = localizacao;
            CapacidadeMaxima = capacidadeMaxima;
            Disponivel = true;
        }

        public void AdicionarEquipamento(Equipamento equipamento) => equipamentos.Add(equipamento);

        public void AdicionarReserva(Reserva reserva) => reservas.Add(reserva);

        public bool VerificarDisponibilidade(DateTime data, TimeSpan inicio, TimeSpan fim)
        {
            if (!Disponivel) return false;
            if (fim <= inicio) return false;

            foreach (Reserva r in reservas)
            {
                // Reservas canceladas ou expiradas não ocupam o espaço
                if (r.Estado == EstadoReserva.Cancelada || r.Estado == EstadoReserva.Expirada)
                    continue;

                bool mesmoDia = r.Data.Date == data.Date;
                bool sobrepoe = inicio < r.HoraFim && fim > r.HoraInicio;
                if (mesmoDia && sobrepoe) return false;
            }
            return true;
        }
    }
}
