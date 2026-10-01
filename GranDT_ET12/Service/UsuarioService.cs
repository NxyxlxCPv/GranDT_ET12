using GranDT_ET12.Service.interfaces;
using GranDT_ET12.Configuracion;
using GranDT_ET12.Modelos;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Seguridad;

namespace GranDT_ET12.Service
{
    public class UsuarioService : IUsuarioService
    {
        private IUsuarioRepositorio _usuarioRepositorio;

        public UsuarioService(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        // Recibe la contraseña sin encriptar, la encripta y guarda el usuario. Devuelve el Id nuevo.
        public int Registrar(Usuario usuario, string passwordSinEncriptar)
        {
            if (usuario.Nombre == "" || usuario.Apellido == "")
            {
                throw new ReglaNegocioException("El nombre y el apellido son obligatorios.");
            }
            if (usuario.Email == "")
            {
                throw new ReglaNegocioException("El email es obligatorio.");
            }
            if (passwordSinEncriptar == "")
            {
                throw new ReglaNegocioException("La contraseña es obligatoria.");
            }
            Usuario? usuarioConEseEmail = _usuarioRepositorio.ObtenerPorEmail(usuario.Email);
            if (usuarioConEseEmail != null)
            {
                throw new ReglaNegocioException("Ya existe un usuario con ese email.");
            }

            int cantidadUsuarios = _usuarioRepositorio.ObtenerTodos().Count();
            if (cantidadUsuarios >= Constantes.MaxUsuarios)
            {
                throw new ReglaNegocioException("Se llego al maximo de usuarios (" + Constantes.MaxUsuarios + ").");
            }

            usuario.Password = Encriptador.EncriptarPassword(passwordSinEncriptar);
            return _usuarioRepositorio.InsertarUsuario(usuario);
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _usuarioRepositorio.ObtenerPorId(id);
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            return _usuarioRepositorio.ObtenerTodos();
        }
    }
}
