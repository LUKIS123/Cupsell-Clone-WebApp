using CupsellCloneAPI.Core.Utils.EmailCommunication;
using CupsellCloneAPI.Core.Utils.EmailCommunication.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CupsellCloneAPI.Core.Utils.Encryption;
using CupsellCloneAPI.Core.Utils.Encryption.Settings;
using System.Text;
using CupsellCloneAPI.Core.Utils.Accessors;

namespace CupsellCloneAPI.Core.Utils
{
    public static class DependencyInjectionUtilitiesExtension
    {
        private const string EmailSectionName = "EmailCommunicationSettings";
        private const string EncryptionSectionName = "EncryptionParams";

        public static IServiceCollection AddUtilities(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
        )
        {
            var result = bool.TryParse(configuration[$"{EmailSectionName}:EnableApiEmailCommunication"],
                out var condition);
            if (result && condition)
            {
                var settings = new ApiEmailSettings();
                configuration.GetSection($"{EmailSectionName}:ApiCommunicationSettings").Bind(settings);
                serviceCollection.AddSingleton(settings);
                serviceCollection.AddScoped<IEmailCommunicationUtility, ApiEmailCommunicationUtility>();
            }
            else
            {
                var settings = new SmtpEmailSettings();
                configuration.GetSection($"{EmailSectionName}:SmtpCommunicationSettings").Bind(settings);
                serviceCollection.AddSingleton(settings);
                serviceCollection.AddScoped<IEmailCommunicationUtility, SmtpEmailCommunicationUtility>();
            }

            var encryptionSettings = new EncryptionSettings
            {
                EncryptionKey = configuration[$"{EncryptionSectionName}:EncryptionKey"] ??
                                throw new InvalidOperationException(),
                Salt = Encoding.ASCII.GetBytes(configuration[$"{EncryptionSectionName}:Salt"] ??
                                               throw new InvalidOperationException())
            };
            serviceCollection.AddSingleton(encryptionSettings);
            serviceCollection.AddScoped<IEncryptionHelper, DefaultEncryptionHelper>();

            return serviceCollection;
        }

        public static IServiceCollection AddAccessors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUrlAccessor, UrlAccessor>();
            serviceCollection.AddScoped<IUserAccessor, UserAccessor>();
            return serviceCollection;
        }
    }
}