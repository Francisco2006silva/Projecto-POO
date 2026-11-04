using System;

namespace GestaoEspacosDesportivos
{
    public class Equipamento
    {
        public int Id { get; private set; }
        public string Nome { get; set; }
        public int QuantidadeDisponivel { get; private set; }
        public string Estado { get; set; } // ex: "Novo", "Usado", "Danificado"

        public Equipamento(int id, string nome, int quantidadeDisponivel, string estado)
        {
            Id = id;
            Nome = nome;
            QuantidadeDisponivel = quantidadeDisponivel;
            Estado = estado;
        }

        public void AtualizarQuantidade(int quantidade)
        {
            if (quantidade < 0)
            {
                Console.WriteLine("A quantidade não pode ser negativa.");
                return;
            }
            QuantidadeDisponivel = quantidade;
        }
    }
}
