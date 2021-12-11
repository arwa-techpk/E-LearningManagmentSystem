//using ELMCOM.Application.Interfaces.CacheRepositories;
using AutoMapper;
using ELMCOM.Application.Interfaces.Contexts;
using ELMCOM.Application.Interfaces.Repositories;
//using ELMCOM.Infrastructure.CacheRepositories;
using ELMCOM.Infrastructure.DbContexts;
using ELMCOM.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ELMCOM.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            #region Repositories

            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));


            services.AddTransient<IUnitOfWork, UnitOfWork>();

            #endregion Repositories
        }
    }
}