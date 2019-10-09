using AkkaTesting.Domain.Entity;
using System.Threading.Tasks;

namespace AkkaTesting.Domain.Interfaces.Repository
{
    public interface IClientRepository
    {
        Task<int> SaveAsync(Client client);
    }
}
