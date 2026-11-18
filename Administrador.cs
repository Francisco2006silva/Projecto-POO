using System;

namespace GestaoEspacosDesportivos
{
    public class Administrador : Utilizador
    {
        public Administrador(int id, string nome, string email, string password, string perfil) : base(id, nome, email, password, perfil)
        {
        }

        public void GerirUtilizadores()
        {
            // TODO: implementar
            throw new NotImplementedException();
        }

        public void GerirEspacos()
        {
            // TODO: implementar
            throw new NotImplementedException();
        }

        public void ConfirmarReserva(Reserva reserva)
        {
            // TODO: implementar
            throw new NotImplementedException();
        }

        public void GerarRelatorios()
        {
            // TODO: implementar
            throw new NotImplementedException();
        }
    }
}