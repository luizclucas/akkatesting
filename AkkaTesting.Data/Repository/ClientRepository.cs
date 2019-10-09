using AkkaTesting.Domain.Entity;
using AkkaTesting.Domain.Interfaces.Repository;
using Dapper;
using System.Threading.Tasks;

namespace AkkaTesting.Data.Repository
{
    public class ClientRepository : IClientRepository
    {
        private DataFactory _data;

        public ClientRepository(DataFactory data)
        {
            _data = data;     
        }

        private static string SaveClient = @"
INSERT IGNORE INTO Client(Id,Name,CPF,City)
VALUES(@Id,@Name,@CPF,@City)
".Trim();
        public async Task<int> SaveAsync(Client client)
        {
            using (var cn = await _data.OpenConnectionAsync())
            {
                var success = await cn.ExecuteAsync(SaveClient, new { client.Id, client.Name, client.CPF, client.City });
                return success;
            }       

        }
    }
}
