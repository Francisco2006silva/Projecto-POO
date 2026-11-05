using System;
using System.Collections.Generic;

namespace GestaoEspacosDesportivos
{
    public class EspacoDesportivo
    {
        // Atributos do diagrama (todos privados: "-")
        private int id;
        private string nome;
        private string tipo;
        private string localizacao;
        private int capacidadeMaxima;
        private bool disponivel;

        // Agregação "possui": 1 EspacoDesportivo -> 0..* Equipamento
        private readonly List<Equipamento> equipamentos = new List<Equipamento>();

        // Associação "recebe": 1 EspacoDesportivo -> 0..* Reserva
        private readonly List<Reserva> reservas = new List<Reserva>();

        // Acesso de leitura aos atributos (as listas não podem ser alteradas de fora)
        public int Id => id;
        public string Nome => nome;
        public string Tipo => tipo;
        public string Localizacao => localizacao;
        public int CapacidadeMaxima => capacidadeMaxima;
        public bool Disponivel => disponivel;
        public IReadOnlyList<Equipamento> Equipamentos => equipamentos;
        public IReadOnlyList<Reserva> Reservas => reservas;

        public EspacoDesportivo(int id, string nome, string tipo, string localizacao, int capacidadeMaxima)
        {
            this.id = id;
            this.nome = nome;
            this.tipo = tipo;
            this.localizacao = localizacao;
            this.capacidadeMaxima = capacidadeMaxima;
            this.disponivel = true;
        }

        // Métodos de apoio às associações (não estão no diagrama, mas são necessários
        // porque as listas são privadas)
        public void AdicionarEquipamento(Equipamento equipamento)
        {
            if (equipamento == null) throw new ArgumentNullException(nameof(equipamento));
            equipamentos.Add(equipamento);
        }

        public void AdicionarReserva(Reserva reserva)
        {
            if (reserva == null) throw new ArgumentNullException(nameof(reserva));
            if (!reservas.Contains(reserva)) reservas.Add(reserva);
        }

        // +verificarDisponibilidade(data: Date, inicio: Time, fim: Time): bool
        public bool VerificarDisponibilidade(DateTime data, TimeSpan inicio, TimeSpan fim)
        {
            if (!disponivel) return false;
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
