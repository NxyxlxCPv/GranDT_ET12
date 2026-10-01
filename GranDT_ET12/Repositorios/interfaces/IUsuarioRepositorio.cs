using GranDT_ET12.Modelos;

namespace GranDT_ET12.Repositorios.interfaces
{
    public interface IUsuarioRepositorio
    {
        int InsertarUsuario(Usuario usuario);
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        IEnumerable<Usuario> ObtenerTodos();
        Usuario? ObtenerPorCredenciales(string email, string passwordEncriptada);
    }
}
