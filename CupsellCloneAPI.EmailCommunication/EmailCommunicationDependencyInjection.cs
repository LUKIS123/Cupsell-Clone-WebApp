using CupsellCloneAPI.EmailCommunication.EmailCommunication;
using CupsellCloneAPI.EmailCommunication.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CupsellCloneAPI.EmailCommunication
{
    public static class EmailCommunicationDependencyInjection
    {
        private const string EmailSectionName = "EmailCommunicationSettings";

        public static IServiceCollection AddEmailCommunication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var result = bool.TryParse(configuration[$"{EmailSectionName}:EnableApiEmailCommunication"],
                out var condition);
            if (result && condition)
            {
                var settings = new ApiEmailSettings();
                configuration.GetSection($"{EmailSectionName}:ApiCommunicationSettings").Bind(settings);
                services.AddSingleton(settings);
                services.AddScoped<IEmailCommunicationUtility, ApiEmailCommunicationUtility>();
            }
            else
            {
                var settings = new SmtpEmailSettings();
                configuration.GetSection($"{EmailSectionName}:SmtpCommunicationSettings").Bind(settings);
                services.AddSingleton(settings);
                services.AddScoped<IEmailCommunicationUtility, SmtpEmailCommunicationUtility>();
            }

            return services;
        }
    }
}