using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Infrastructure.Enums;
using ECommerceAPI.Infrastructure.Services.Storage;
using ECommerceAPI.Infrastructure.Services.Storage.Azure;
using ECommerceAPI.Infrastructure.Services.Storage.Local;
using ECommerceAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollections)
        {
            serviceCollections.AddScoped<IStorageService, StorageService>();
            serviceCollections.AddScoped<ITokenHandler, TokenHandler>();
        }

        public static void AddStorage<T> (this IServiceCollection serviceCollections) where T : Storage, IStorage
        {
            serviceCollections.AddScoped<IStorage, T>();
        }

        //Alternatif yol, ama doğrusu yukarıdaki gibi generic kullanmak
        public static void AddStorage(this IServiceCollection serviceCollections, StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    serviceCollections.AddScoped<IStorage, LocalStorage>();
                    break;
                case StorageType.Azure:
                    serviceCollections.AddScoped<IStorage, AzureStorage>();
                    break;
                case StorageType.AWS:
                    break;
                case StorageType.GoogleCloud:
                    break;
                default:
                    serviceCollections.AddScoped<IStorage, LocalStorage>();
                    throw new NotImplementedException();
            }
        }
    }
}
