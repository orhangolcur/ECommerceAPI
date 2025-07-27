using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            
            services.AddMediatR(typeof(ServiceRegistration)); // bu assembly'deki(application) tüm MediatR handler'larını kaydeder. örneğin products için GetAllProductsQueryHandler gibi
            services.AddHttpClient(); 

        }
    }
}
