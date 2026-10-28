using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GestaoEspacosDesportivos
{
    // Classe Cliente
    public class Cliente
    {
        public int id;
        public string? nome;
        public int idade;
        public string? telefone;
    }
    //---------------------------------------------------------------------------
    // Classe Instalação Desportiva / Espaço
    public class Instalacao
    {
        public string? nome;
        public string? modalidade;
        public double precoPorHora;
        public double descontoAtleta; // Percentagem de desconto (ex: 10%)
    }
    //---------------------------------------------------------------------------

    // Classe Fatura / Detalhes de Pagamento
    public class Fatura
    {
        public double valorTotal;
        public double valorDesconto;
        public double valorFinal;
    }
    //---------------------------------------------------------------------------

    // Classe Reserva
    public class Reserva
    {
        public Cliente? cliente;
        public Instalacao? instalacao;
        public DateTime dataHora;
        public int duracaoHoras;
        public Fatura? fatura;
        public string? estado; // "Confirmada" ou "Cancelada"
    }
    //---------------------------------------------------------------------------

    internal class Program
    {
        static void Main(string[] args)
        {
            List<Cliente> listaClientes = new List<Cliente>();
            List<Reserva> historicoReservas = new List<Reserva>();

            Dictionary<string, List<Instalacao>> dicionarioInstalacoesPorModalidade = CarregarInstalacoesDoFicheiro("instalacoes.txt");

            // IMPORTAR DADOS LOGO AO INICIAR
            ImportarClientes("clientes.txt", listaClientes);
            ImportarHistoricoReservas("reservas.txt", historicoReservas, listaClientes, dicionarioInstalacoesPorModalidade);

            bool sairPrograma = false;
            while (!sairPrograma)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n===== GESTÃO DE RESERVAS DE ESPAÇOS DESPORTIVOS =====");
                Console.ResetColor();
                Console.WriteLine("1. Gestão de Clientes (Registar/Listar/Editar/Remover)");
                Console.WriteLine("2. Criar Nova Reserva");
                Console.WriteLine("3. Ver Estatísticas e Exportar Relatórios");
                Console.WriteLine("0. Sair");
                Console.Write("Escolha uma opção: ");

                string opcaoPrincipal = Console.ReadLine() ?? "";

                switch (opcaoPrincipal)
                {
                    case "1":
                        MenuGestaoClientes(listaClientes);
                        break;

                    case "2":
                        if (listaClientes.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n[AVISO] Registre um cliente primeiro na opção 1!");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("\n--- QUEM VAI REALIZAR A RESERVA? ---");
                            Console.ResetColor();
                            for (int i = 0; i < listaClientes.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {listaClientes[i].nome}");
                            }
                            Console.WriteLine("0. Voltar ao Menu Principal");
                            Console.Write("Número do cliente: ");

                            string entradaCliente = Console.ReadLine() ?? "";

                            if (int.TryParse(entradaCliente, out int numeroDigitado))
                            {
                                if (numeroDigitado == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("Retornando ao menu principal...");
                                    Console.ResetColor();
                                }
                                else if (numeroDigitado > 0 && numeroDigitado <= listaClientes.Count)
                                {
                                    Cliente clienteSelecionado = listaClientes[numeroDigitado - 1];
                                    ExecutarReserva(clienteSelecionado, historicoReservas, dicionarioInstalacoesPorModalidade);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Opção Inválida! Esse número de cliente não existe.");
                                    Console.ResetColor();
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Opção Inválida! Por favor, digite um número.");
                                Console.ResetColor();
                            }
                        }
                        break;

                    case "3":
                        MostrarEstatisticas(historicoReservas, listaClientes, dicionarioInstalacoesPorModalidade);
                        break;

                    case "0":
                        sairPrograma = true;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opção inválida.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // --------------- GESTÃO DE CLIENTES -------------------
        static void MenuGestaoClientes(List<Cliente> lista)
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n--- GESTÃO DE CLIENTES ---");
                Console.ResetColor();
                Console.WriteLine("1. Registar Novo Cliente");
                Console.WriteLine("2. Listar Clientes");
                Console.WriteLine("3. Editar Cliente");
                Console.WriteLine("4. Remover Cliente");
                Console.WriteLine("0. Voltar ao Menu Principal");
                Console.Write("Opção: ");

                string op = Console.ReadLine() ?? "";

                switch (op)
                {
                    case "1":
                        Cliente novo = lerDadosCliente(lista);
                        lista.Add(novo);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Cliente registado com sucesso!");
                        Console.ResetColor();
                        break;

                    case "2":
                        if (lista.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Lista vazia.");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("\n--- CLIENTES REGISTADOS ---");
                            Console.ResetColor();
                            foreach (Cliente c in lista)
                                Console.WriteLine($"ID: {c.id} | Nome: {c.nome} | Idade: {c.idade} | Telefone: {c.telefone}");
                        }
                        break;

                    case "3":
                        Console.Write("Introduza o ID do cliente que deseja editar: ");
                        if (int.TryParse(Console.ReadLine(), out int idBusca))
                        {
                            Cliente? clienteEditar = lista.FirstOrDefault(c => c.id == idBusca);
                            if (clienteEditar != null)
                            {
                                Console.Write("Novo nome: ");
                                clienteEditar.nome = Console.ReadLine();
                                Console.Write("Nova idade: ");
                                int.TryParse(Console.ReadLine(), out clienteEditar.idade);
                                Console.Write("Novo telefone: ");
                                clienteEditar.telefone = Console.ReadLine();

                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.WriteLine("Cliente editado com sucesso!");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Cliente com esse ID não foi encontrado.");
                                Console.ResetColor();
                            }
                        }
                        break;

                    case "4":
                        Console.Write("ID do cliente a remover: ");
                        if (int.TryParse(Console.ReadLine(), out int idRemover))
                        {
                            bool existeCliente = lista.Any(c => c.id == idRemover);

                            if (!existeCliente)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Não encontrado.");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"Tem certeza que deseja remover o cliente com ID {idRemover}? (s/n): ");
                                Console.ResetColor();

                                string resposta = Console.ReadLine() ?? "";

                                if (resposta.ToLower() == "s")
                                {
                                    lista.RemoveAll(c => c.id == idRemover);
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine("Removido com sucesso!");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("Remoção cancelada pelo utilizador.");
                                    Console.ResetColor();
                                }
                            }
                        }
                        break;

                    case "0":
                        voltar = true;
                        break;
                }
            }
        }
        //---------------------------------------------------------------------------

        static Cliente lerDadosCliente(List<Cliente> lista)
        {
            Cliente c = new Cliente();
            c.id = lista.Count > 0 ? lista.Max(x => x.id) + 1 : 1;

            Console.Write("Nome do Cliente: ");
            c.nome = Console.ReadLine()?.Trim();

            Console.Write("Idade do Cliente: ");
            int.TryParse(Console.ReadLine(), out c.idade);

            Console.Write("Telefone do Cliente: ");
            c.telefone = Console.ReadLine()?.Trim();

            return c;
        }
        //---------------------------------------------------------------------------

        // ---- EXECUÇÃO DE UMA RESERVA ----
        static Reserva? ExecutarReserva(Cliente cliente, List<Reserva> historico, Dictionary<string, List<Instalacao>> instalacoesPorModalidade)
        {
            Reserva novaReserva = new Reserva();
            novaReserva.cliente = defenderClienteNull(cliente);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\nOlá, {cliente.nome}! Vamos agendar o teu espaço desportivo.");
            Console.ResetColor();

            List<Instalacao> listaAuxiliar = new List<Instalacao>();
            foreach (List<Instalacao> lista in instalacoesPorModalidade.Values)
            {
                listaAuxiliar.AddRange(lista);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== ESCOLHA O ESPAÇO DESPORTIVO ===");
            Console.ResetColor();

            int indiceMenu = 1;
            foreach (KeyValuePair<string, List<Instalacao>> par in instalacoesPorModalidade)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--- MODALIDADE: {par.Key.ToUpper()} ---");
                Console.ResetColor();
                foreach (Instalacao inst in par.Value)
                {
                    Console.WriteLine($"{indiceMenu} - {inst.nome} ({inst.precoPorHora}€/hora)");
                    indiceMenu += 1;
                }
            }
            Console.WriteLine("0 - Cancelar Processo");
            Console.Write("Escolha: ");

            string entrada = Console.ReadLine() ?? "";

            if (int.TryParse(entrada, out int escolha) && escolha > 0 && escolha <= listaAuxiliar.Count)
            {
                Instalacao escolhida = listaAuxiliar[escolha - 1];
                novaReserva.instalacao = escolhida;

                Console.Write("Introduza a data (AAAA-MM-DD): ");
                string dataStr = Console.ReadLine() ?? "";

                Console.Write("Introduza a hora (HH:MM): ");
                string horaStr = Console.ReadLine() ?? "";

                if (DateTime.TryParse($"{dataStr} {horaStr}", out DateTime dataHora))
                {
                    novaReserva.dataHora = dataHora;
                }
                else
                {
                    novaReserva.dataHora = DateTime.Now;
                    Console.WriteLine("Data/Hora inválida! Foi assumido a data e hora atual.");
                }

                Console.Write("Duração da reserva (em horas): ");
                int.TryParse(Console.ReadLine(), out int duracao);
                novaReserva.duracaoHoras = duracao > 0 ? duracao : 1;

                // Calcular Fatura
                double valorBruto = escolhida.precoPorHora * novaReserva.duracaoHoras;
                double desconto = valorBruto * (escolhida.descontoAtleta / 100.0);
                
                // Desconto extra se for jovem/júnior (< 18 anos)
                if (cliente.idade < 18)
                {
                    desconto += valorBruto * 0.05; // 5% extra
                }

                double valorFinal = valorBruto - desconto;

                novaReserva.fatura = new Fatura
                {
                    valorTotal = valorBruto,
                    valorDesconto = desconto,
                    valorFinal = valorFinal < 0 ? 0 : valorFinal
                };

                novaReserva.estado = "Confirmada";

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n=== RESERVA CONFIRMADA COM SUCESSO! ===");
                Console.WriteLine($"Espaço: {escolhida.nome}");
                Console.WriteLine($"Data/Hora: {novaReserva.dataHora:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Total a Pagar: {novaReserva.fatura.valorFinal:F2}€ (Desconto de {novaReserva.fatura.valorDesconto:F2}€ aplicado)");
                Console.ResetColor();

                historico.Add(novaReserva);
                Console.WriteLine("\nPressiona qualquer tecla para continuar...");
                Console.ReadKey();

                return novaReserva;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reserva cancelada ou opção inválida.");
                Console.ResetColor();
                return null;
            }
        }
        //---------------------------------------------------------------------------

        // ---- ESTATÍSTICAS E FICHEIROS ----
        static void MostrarEstatisticas(List<Reserva> historicoReservas, List<Cliente> listaClientes, Dictionary<string, List<Instalacao>> instalacoesPorModalidade)
        {
            bool voltarEst = false;
            while (!voltarEst)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n--- MENU DE ESTATÍSTICAS E ANÁLISE ---");
                Console.ResetColor();
                Console.WriteLine("1. Estatísticas Gerais (Totais de Reservas e Receitas)");
                Console.WriteLine("2. Ver Top 3 Clientes com Mais Reservas");
                Console.WriteLine("3. Espaço Mais Reservado");
                Console.WriteLine("4. Pesquisar se um Espaço/Instalação já foi reservado");
                Console.WriteLine("5. Exportar Clientes Alfabeticamente");
                Console.WriteLine("6. Exportar Histórico Geral de Reservas");
                Console.WriteLine("0. Voltar ao Menu Principal");
                Console.Write("Escolha uma opção: ");

                switch (Console.ReadLine() ?? "")
                {
                    case "1":
                        Console.WriteLine($"\nTotal de Reservas: {historicoReservas.Count}");
                        int confirmadas = historicoReservas.Count(r => r.estado == "Confirmada");
                        if (historicoReservas.Count > 0)
                        {
                            double totalFaturado = historicoReservas.Where(r => r.fatura != null).Sum(r => r.fatura!.valorFinal);
                            Console.WriteLine($"Reservas Confirmadas: {confirmadas} ({(double)confirmadas / historicoReservas.Count * 100:F1}%)");
                            Console.WriteLine($"Total Faturado: {totalFaturado:F2}€");
                            Console.WriteLine($"Média de idade dos clientes ativos: {historicoReservas.Average(r => r.cliente!.idade):F1} anos");
                        }
                        break;

                    case "2":
                        var top3 = historicoReservas.Where(r => r.cliente != null)
                                                   .GroupBy(r => r.cliente!.nome)
                                                   .OrderByDescending(g => g.Count())
                                                   .Take(3);
                        Console.WriteLine("\n--- TOP 3 CLIENTES COM MAIS RESERVAS ---");
                        foreach (var g in top3) Console.WriteLine($"{g.Key} - {g.Count()} reservas");
                        break;

                    case "3":
                        if (historicoReservas.Any())
                        {
                            var maisReservado = historicoReservas.GroupBy(r => r.instalacao!.nome)
                                                                .OrderByDescending(g => g.Count())
                                                                .FirstOrDefault();
                            if (maisReservado != null)
                                Console.WriteLine($"\nEspaço mais popular: {maisReservado.Key} ({maisReservado.Count()} reservas)");
                        }
                        else Console.WriteLine("\nNenhuma reserva registada.");
                        break;

                    case "4":
                        Console.Write("\nDigite o nome do espaço a pesquisar: ");
                        string busca = Console.ReadLine()?.ToLower() ?? "";
                        bool usada = historicoReservas.Any(r => r.instalacao != null && r.instalacao.nome!.ToLower().Contains(busca));
                        Console.WriteLine(usada ? "Este espaço já tem histórico de reservas!" : "Espaço nunca reservado.");
                        break;

                    case "5":
                        var ordenados = listaClientes.OrderBy(c => c.nome).Select(c => $"{c.id};{c.nome};{c.idade};{c.telefone}");
                        File.WriteAllLines("clientes_ordenados.txt", ordenados);
                        Console.WriteLine("Ficheiro 'clientes_ordenados.txt' gerado com sucesso!");
                        break;

                    case "6":
                        List<string> linhasHist = new List<string>();
                        foreach (var r in historicoReservas)
                        {
                            linhasHist.Add($"{r.cliente!.nome};{r.estado}");
                            linhasHist.Add($"{r.instalacao!.nome};{r.dataHora:yyyy-MM-dd};{r.dataHora:HH:mm};{r.duracaoHoras}");
                            linhasHist.Add($"{r.fatura!.valorTotal};{r.fatura.valorDesconto};{r.fatura.valorFinal}");
                        }
                        File.WriteAllLines("historico_reservas_exportado.txt", linhasHist);
                        Console.WriteLine("Histórico exportado para 'historico_reservas_exportado.txt'!");
                        break;

                    case "0":
                        voltarEst = true;
                        break;
                }
            }
        }
        //---------------------------------------------------------------------------

        // ---- CARREGAMENTO AUTOMÁTICO DE FICHEIROS ----
        static Dictionary<string, List<Instalacao>> CarregarInstalacoesDoFicheiro(string nomeFicheiro)
        {
            Dictionary<string, List<Instalacao>> dicionarioAux = new Dictionary<string, List<Instalacao>>();

            if (File.Exists(nomeFicheiro))
            {
                string[] linhas = File.ReadAllLines(nomeFicheiro);
                foreach (string linha in linhas)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    string[] partes = linha.Split(';');

                    string modalidade = partes[1].Trim();
                    Instalacao novaInstalacao = new Instalacao
                    {
                        nome = partes[0].Trim(),
                        modalidade = modalidade,
                        precoPorHora = Convert.ToDouble(partes[2]),
                        descontoAtleta = partes.Length > 3 ? Convert.ToDouble(partes[3]) : 0.0
                    };

                    if (!dicionarioAux.ContainsKey(modalidade))
                    {
                        dicionarioAux.Add(modalidade, new List<Instalacao>());
                    }
                    dicionarioAux[modalidade].Add(novaInstalacao);
                }
            }
            return dicionarioAux;
        }
        //---------------------------------------------------------------------------

        static void ImportarClientes(string nomeFicheiro, List<Cliente> lista)
        {
            if (!File.Exists(nomeFicheiro)) return;

            using (StreamReader leitor = new StreamReader(nomeFicheiro))
            {
                string? linha;
                while ((linha = leitor.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    string[] partes = linha.Split(';');

                    lista.Add(new Cliente
                    {
                        id = Convert.ToInt32(partes[0]),
                        nome = partes[1].Trim(),
                        idade = Convert.ToInt32(partes[2]),
                        telefone = partes.Length > 3 ? partes[3].Trim() : ""
                    });
                }
            }
        }
        //---------------------------------------------------------------------------

        static void ImportarHistoricoReservas(string nomeFicheiro, List<Reserva> historico, List<Cliente> clientes, Dictionary<string, List<Instalacao>> instalacoesPorModalidade)
        {
            if (!File.Exists(nomeFicheiro)) return;

            using (StreamReader leitor = new StreamReader(nomeFicheiro))
            {
                string? linha1;
                while ((linha1 = leitor.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha1)) continue;
                    string linha2 = leitor.ReadLine() ?? "";
                    string linha3 = leitor.ReadLine() ?? "";

                    string[] partesLinha1 = linha1.Split(';');
                    string nomeCliente = partesLinha1[0].Trim();
                    string estado = partesLinha1.Length > 1 ? partesLinha1[1].Trim() : "Confirmada";

                    Cliente? clienteEncontrado = clientes.FirstOrDefault(c => c.nome!.Equals(nomeCliente, StringComparison.OrdinalIgnoreCase));

                    if (clienteEncontrado == null)
                    {
                        clienteEncontrado = new Cliente { id = 999, nome = nomeCliente, idade = 20, telefone = "N/A" };
                    }

                    string[] partesLinha2 = linha2.Split(';');
                    string nomeInstalacao = partesLinha2.Length > 0 ? partesLinha2[0].Trim() : "";
                    
                    Instalacao? instalacaoEncontrada = null;
                    foreach (var lista in instalacoesPorModalidade.Values)
                    {
                        var inst = lista.FirstOrDefault(i => i.nome!.Equals(nomeInstalacao, StringComparison.OrdinalIgnoreCase));
                        if (inst != null)
                        {
                            instalacaoEncontrada = inst;
                            break;
                        }
                    }

                    if (instalacaoEncontrada == null)
                    {
                        instalacaoEncontrada = new Instalacao { nome = nomeInstalacao, modalidade = "Geral", precoPorHora = 30.0 };
                    }

                    Reserva novaReserva = new Reserva
                    {
                        cliente = clienteEncontrado,
                        instalacao = instalacaoEncontrada,
                        estado = estado
                    };

                    if (partesLinha2.Length >= 4)
                    {
                        DateTime.TryParse($"{partesLinha2[1]} {partesLinha2[2]}", out DateTime dt);
                        novaReserva.dataHora = dt;
                        novaReserva.duracaoHoras = Convert.ToInt32(partesLinha2[3]);
                    }

                    // Reconstruir Fatura (Linha 3)
                    string[] partesLinha3 = linha3.Split(';');
                    if (partesLinha3.Length >= 3)
                    {
                        novaReserva.fatura = new Fatura
                        {
                            valorTotal = Convert.ToDouble(partesLinha3[0]),
                            valorDesconto = Convert.ToDouble(partesLinha3[1]),
                            valorFinal = Convert.ToDouble(partesLinha3[2])
                        };
                    }

                    historico.Add(novaReserva);
                }
            }
        }

        private static Cliente defenderClienteNull(Cliente c) => c ?? new Cliente { nome = "Anónimo", idade = 20, telefone = "N/A" };
    }
}
