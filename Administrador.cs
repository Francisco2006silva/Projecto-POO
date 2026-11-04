using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoEspacosDesportivos
{
    public class Administrador : Utilizador
    {
        public Administrador(int id, string nome, string email, string password)
            : base(id, nome, email, password, "Administrador")
        {
        }

        // ---------------- GERIR UTILIZADORES ----------------
        public void GerirUtilizadores(List<Utilizador> utilizadores)
        {
            bool voltar = false;
            while (!voltar)
            {
                Consola.Mensagem("\n--- GERIR UTILIZADORES ---", ConsoleColor.Green);
                Console.WriteLine("1. Listar Utilizadores");
                Console.WriteLine("2. Registar Utilizador Normal");
                Console.WriteLine("3. Remover Utilizador");
                Console.WriteLine("0. Voltar");

                switch (Consola.LerTexto("Opção: "))
                {
                    case "1":
                        foreach (Utilizador u in utilizadores)
                            Console.WriteLine($"ID: {u.Id} | {u.Nome} | {u.Email} | {u.Perfil}");
                        break;

                    case "2":
                        string nome = Consola.LerTexto("Nome: ");
                        string email = Consola.LerTexto("Email: ");
                        string password = Consola.LerTexto("Password: ");

                        if (nome == "" || email == "" || password == "")
                        {
                            Consola.Mensagem("Todos os campos são obrigatórios.", ConsoleColor.Red);
                            break;
                        }
                        if (utilizadores.Any(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                        {
                            Consola.Mensagem("Já existe um utilizador com esse email.", ConsoleColor.Red);
                            break;
                        }

                        int novoId = utilizadores.Count > 0 ? utilizadores.Max(x => x.Id) + 1 : 1;
                        utilizadores.Add(new UtilizadorNormal(novoId, nome, email, password));
                        Consola.Mensagem("Utilizador registado com sucesso!", ConsoleColor.DarkGreen);
                        break;

                    case "3":
                        int idRemover = Consola.LerInteiro("ID do utilizador a remover: ");
                        Utilizador? alvo = utilizadores.FirstOrDefault(x => x.Id == idRemover);

                        if (alvo == null)
                        {
                            Consola.Mensagem("Utilizador não encontrado.", ConsoleColor.Red);
                        }
                        else if (alvo == this)
                        {
                            Consola.Mensagem("Não pode remover a sua própria conta.", ConsoleColor.Red);
                        }
                        else if (Consola.LerTexto($"Remover '{alvo.Nome}'? (s/n): ").ToLower() == "s")
                        {
                            // Cancela as reservas ativas do utilizador removido
                            if (alvo is UtilizadorNormal normal)
                            {
                                foreach (Reserva r in normal.ConsultarReservas().Where(res => res.EstaAtiva))
                                    r.Cancelar();
                            }
                            utilizadores.Remove(alvo);
                            Consola.Mensagem("Utilizador removido.", ConsoleColor.DarkGreen);
                        }
                        break;

                    case "0":
                        voltar = true;
                        break;
                }
            }
        }

        // ---------------- GERIR ESPAÇOS ----------------
        public void GerirEspacos(List<EspacoDesportivo> espacos)
        {
            bool voltar = false;
            while (!voltar)
            {
                Consola.Mensagem("\n--- GERIR ESPAÇOS ---", ConsoleColor.Green);
                Console.WriteLine("1. Listar Espaços");
                Console.WriteLine("2. Adicionar Espaço");
                Console.WriteLine("3. Alterar Disponibilidade de um Espaço");
                Console.WriteLine("4. Adicionar Equipamento a um Espaço");
                Console.WriteLine("5. Atualizar Quantidade de um Equipamento");
                Console.WriteLine("6. Remover Espaço");
                Console.WriteLine("0. Voltar");

                switch (Consola.LerTexto("Opção: "))
                {
                    case "1":
                        ListarEspacos(espacos);
                        break;

                    case "2":
                        string nome = Consola.LerTexto("Nome: ");
                        string tipo = Consola.LerTexto("Tipo (ex: Futebol, Ténis): ");
                        string localizacao = Consola.LerTexto("Localização: ");
                        int capacidade = Consola.LerInteiro("Capacidade máxima: ", 1);

                        if (nome == "")
                        {
                            Consola.Mensagem("O nome é obrigatório.", ConsoleColor.Red);
                            break;
                        }
                        int novoId = espacos.Count > 0 ? espacos.Max(e => e.Id) + 1 : 1;
                        espacos.Add(new EspacoDesportivo(novoId, nome, tipo, localizacao, capacidade));
                        Consola.Mensagem("Espaço adicionado com sucesso!", ConsoleColor.DarkGreen);
                        break;

                    case "3":
                        EspacoDesportivo? espacoDisp = EscolherEspaco(espacos);
                        if (espacoDisp != null)
                        {
                            espacoDisp.Disponivel = !espacoDisp.Disponivel;
                            Console.WriteLine($"'{espacoDisp.Nome}' está agora {(espacoDisp.Disponivel ? "disponível" : "indisponível")}.");
                        }
                        break;

                    case "4":
                        EspacoDesportivo? espacoEquip = EscolherEspaco(espacos);
                        if (espacoEquip != null)
                        {
                            string nomeEquip = Consola.LerTexto("Nome do equipamento: ");
                            int qtd = Consola.LerInteiro("Quantidade disponível: ", 0);
                            string estadoEquip = Consola.LerTexto("Estado (ex: Novo, Usado): ");

                            int idEquip = espacos.SelectMany(e => e.Equipamentos)
                                                 .Select(q => q.Id).DefaultIfEmpty(0).Max() + 1;
                            espacoEquip.AdicionarEquipamento(new Equipamento(idEquip, nomeEquip, qtd, estadoEquip));
                            Consola.Mensagem("Equipamento adicionado!", ConsoleColor.DarkGreen);
                        }
                        break;

                    case "5":
                        EspacoDesportivo? espacoQtd = EscolherEspaco(espacos);
                        if (espacoQtd != null)
                        {
                            if (espacoQtd.Equipamentos.Count == 0)
                            {
                                Console.WriteLine("Este espaço não tem equipamentos.");
                                break;
                            }
                            foreach (Equipamento q in espacoQtd.Equipamentos)
                                Console.WriteLine($"ID: {q.Id} | {q.Nome} | Qtd: {q.QuantidadeDisponivel} | {q.Estado}");

                            int idQ = Consola.LerInteiro("ID do equipamento: ");
                            Equipamento? equip = espacoQtd.Equipamentos.FirstOrDefault(q => q.Id == idQ);
                            if (equip == null)
                            {
                                Consola.Mensagem("Equipamento não encontrado.", ConsoleColor.Red);
                                break;
                            }
                            equip.AtualizarQuantidade(Consola.LerInteiro("Nova quantidade: "));
                            Console.WriteLine("Quantidade atualizada.");
                        }
                        break;

                    case "6":
                        EspacoDesportivo? espacoRem = EscolherEspaco(espacos);
                        if (espacoRem != null)
                        {
                            if (espacoRem.Reservas.Any(r => r.EstaAtiva))
                                Consola.Mensagem("Não é possível remover: o espaço tem reservas ativas.", ConsoleColor.Red);
                            else if (Consola.LerTexto($"Remover '{espacoRem.Nome}'? (s/n): ").ToLower() == "s")
                            {
                                espacos.Remove(espacoRem);
                                Consola.Mensagem("Espaço removido.", ConsoleColor.DarkGreen);
                            }
                        }
                        break;

                    case "0":
                        voltar = true;
                        break;
                }
            }
        }

        private static void ListarEspacos(List<EspacoDesportivo> espacos)
        {
            if (espacos.Count == 0)
            {
                Console.WriteLine("Não existem espaços registados.");
                return;
            }
            foreach (EspacoDesportivo e in espacos)
            {
                Console.WriteLine($"ID: {e.Id} | {e.Nome} | {e.Tipo} | {e.Localizacao} | Cap: {e.CapacidadeMaxima} | " +
                                  $"{(e.Disponivel ? "Disponível" : "Indisponível")} | Equipamentos: {e.Equipamentos.Count}");
            }
        }

        private static EspacoDesportivo? EscolherEspaco(List<EspacoDesportivo> espacos)
        {
            ListarEspacos(espacos);
            if (espacos.Count == 0) return null;

            int id = Consola.LerInteiro("ID do espaço: ");
            EspacoDesportivo? espaco = espacos.FirstOrDefault(e => e.Id == id);
            if (espaco == null)
                Consola.Mensagem("Espaço não encontrado.", ConsoleColor.Red);
            return espaco;
        }

        // ---------------- CONFIRMAR RESERVA ----------------
        public void ConfirmarReserva(Reserva reserva)
        {
            reserva.Confirmar();
        }

        // ---------------- GERAR RELATÓRIOS ----------------
        // Dependência "consulta": o Administrador usa a classe Relatorio
        public void GerarRelatorios(List<EspacoDesportivo> espacos)
        {
            Relatorio relatorio = new Relatorio(espacos);

            Consola.Mensagem("\n=== RELATÓRIOS ===", ConsoleColor.Cyan);
            Console.WriteLine($"Total de reservas: {relatorio.TotalReservas()}");
            relatorio.ReservasPorEspaco();
            Console.WriteLine($"\nTaxa de ocupação: {relatorio.CalcularTaxaOcupacao():F1}%");

            if (Consola.LerTexto("\nVer reservas por período? (s/n): ").ToLower() == "s")
            {
                DateTime inicio = Consola.LerData("Data inicial (AAAA-MM-DD): ");
                DateTime fim = Consola.LerData("Data final (AAAA-MM-DD): ");
                relatorio.ReservasPorPeriodo(inicio, fim);
            }
        }
    }
}
