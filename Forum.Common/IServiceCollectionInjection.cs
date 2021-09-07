using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Common
{
    public static class IServiceCollectionInjection
    {
        public static IServiceCollection AddServiceExtension(this IServiceCollection services)
        {
            services.AddScoped<EmailHelper, EmailHelper>();
            services.AddScoped<EmailSentStatus>();
            return services;
        }
    }
}
