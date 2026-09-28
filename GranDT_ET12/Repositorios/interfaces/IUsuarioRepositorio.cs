using GranDT_ET12.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IUsuarioRepositorio
    {
        int InsertarUsuario(Usuario usuario);
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        IEnumerable<Usuario> ObtenerTodos();
        bool ExisteEmail(string email);
    }
}
