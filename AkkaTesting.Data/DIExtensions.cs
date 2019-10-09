using AkkaTesting.Data.Repository;
using AkkaTesting.Domain.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaTesting.Data
{
    public static class DIExtensions
    {
        public static void AddData(this IServiceCollection services)
        {
            services.AddSingleton<DataFactory>();
            services.AddTransient<IClientRepository, ClientRepository>();
        }
    }
}
