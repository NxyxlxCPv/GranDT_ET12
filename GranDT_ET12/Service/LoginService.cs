using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Seguridad;

namespace GranDT_ET12.Service
{
    public class LoginService : ILoginService
    {
        private IUsuarioRepositorio _usuarioRepositorio;

        public LoginService(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        // Devuelve el usuario si el email y la contraseña son correctos, sino devuelve null
        public Usuario? Login(string email, string password)
        {
            if (email == "" || password == "")
            {
                throw new ReglaNegocioException("El email y la contraseña son obligatorios.");
            }

            string passwordEncriptada = Encriptador.EncriptarPassword(password);
            return _usuarioRepositorio.ObtenerPorCredenciales(email, passwordEncriptada);
        }
    }
}
