using System.Threading.Tasks;

namespace OdooDotNetClient
{
    public interface IOdooClient
    {
        Task<int?> AuthenticateAsync();
    }
}
