using System;

namespace GestaoEspacosDesportivos
{
    // Classe abstrata: não se criam Utilizadores diretamente,
    // apenas Administradores ou UtilizadoresNormais.
    public abstract class Utilizador
    {
        private string password;

        public int Id { get; private set; }
        public string Nome { get; set; }
        public string Email { get; private set; }
        public string Perfil { get; private set; }

        protected Utilizador(int id, string nome, string email, string password, string perfil)
        {
            Id = id;
            Nome = nome;
            Email = email;
            this.password = password;
            Perfil = perfil;
        }

        public bool IniciarSessao(string email, string password)
        {
            return Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                   && this.password == password;
        }

        public void ConsultarPerfil()
        {
            Console.WriteLine("\n--- O MEU PERFIL ---");
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Nome: {Nome}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Perfil: {Perfil}");
        }
    }
}
