using WebApp.Models;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IAuthService
    {
        Task<Usuario?> Authenticate(string email, string password);
        Task<Usuario?> Register(Usuario usuario, string password);
        Task<bool> EmailExists(string email);
    }
}