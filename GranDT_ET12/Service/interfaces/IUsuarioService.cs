using GranDT_ET12.Modelos;

namespace GranDT_ET12.Service.interfaces
{
    public interface IUsuarioService
    {
        int Registrar(Usuario usuario, string passwordSinEncriptar);
        Usuario? ObtenerPorId(int id);
        IEnumerable<Usuario> ObtenerTodos();
    }
}
