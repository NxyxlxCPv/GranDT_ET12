using GranDT_ET12.Modelos;

namespace GranDT_ET12.Service.interfaces
{
    public interface ILoginService
    {
        Usuario? Login(string email, string password);
    }
}
