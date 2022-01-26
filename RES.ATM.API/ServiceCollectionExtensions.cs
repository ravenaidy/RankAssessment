using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.Domain.Account.Services;
using RES.ATM.API.Domain.ATM.Contracts;
using RES.ATM.API.Domain.ATM.Services;
using RES.ATM.API.Domain.Contracts;
using RES.ATM.API.Infrastructure.Repositories.Account;

namespace RES.ATM.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddATMServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IATMService>(sp => new ATMService(configuration.GetValue<decimal>("ATMAmount")));
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IAccountService, AccountService>();            
            return services;
        }
    }
}
