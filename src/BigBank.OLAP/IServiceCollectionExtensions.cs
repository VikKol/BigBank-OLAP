using System;
using BigBank.OLAP.Extensions;
using BigBank.OLAP.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Raygun.Druid4Net;

namespace BigBank.OLAP
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBigBankOlapServices(this IServiceCollection services, Uri druidHostUri)
        {
            services.AddSingleton<IDruidClient>(
                sp => new DruidClient(new NewtonsoftSerializer(), druidHostUri.OmitPort().AbsoluteUri.TrimEnd('/'), druidHostUri.Port));

            services.AddSingleton<IPriceCalculationService, PriceCalculationService>();

            return services;
        }
    }
}