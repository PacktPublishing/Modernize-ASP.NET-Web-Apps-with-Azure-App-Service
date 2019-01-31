using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcOrderService
{
    public class ServiceProviderBuilder : IServiceProviderBuilder
    {
        public IServiceProvider BuildServiceProvider()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("MusicStoreEntities");

            var services = new ServiceCollection();
            services.AddDbContext<MusicStoreEntities>(options => options.UseSqlServer(connectionString));

            return services.BuildServiceProvider(true);
        }
    }

    public interface IServiceProviderBuilder
    {
        //IContainerBuilder RegisterModule(IModule module = null);

        IServiceProvider BuildServiceProvider();
    }
}
