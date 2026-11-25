using System;

namespace GestaoEspacosDesportivos
{
    public class Equipamento
    {
        private int id;
        private string nome;
        private int quantidadeDisponivel;
        private string estado;

        public int Id => id;
        public string Nome => nome;
        public int QuantidadeDisponivel => quantidadeDisponivel;
        public string Estado => estado;

        public Equipamento(int id, string nome, int quantidadeDisponivel)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do equipamento é obrigatório.");
            if (quantidadeDisponivel < 0)
                throw new ArgumentException("A quantidade inicial não pode ser negativa.");

            this.id = id;
            this.nome = nome;
            this.quantidadeDisponivel = quantidadeDisponivel;
            this.estado = quantidadeDisponivel > 0 ? "Disponível" : "Indisponível";
        }

        public void AtualizarQuantidade(int quantidade)
        {
            if (quantidade < 0)
                throw new ArgumentException("A quantidade disponível não pode ser negativa.");

            quantidadeDisponivel = quantidade;
            estado = quantidadeDisponivel > 0 ? "Disponível" : "Indisponível";
        }

        public override string ToString()
        {
            return $"#{Id} | {Nome} | Qtd: {QuantidadeDisponivel} | {Estado}";
        }
    }
}