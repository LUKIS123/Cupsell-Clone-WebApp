using CupsellCloneAPI.Core.Utils.EmailCommunication;
using CupsellCloneAPI.Core.Utils.EmailCommunication.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using CupsellCloneAPI.Core.Utils.Encryption;
using CupsellCloneAPI.Core.Utils.Encryption.Settings;
using System.Text;

namespace CupsellCloneAPI.Core.Utils
{
    public static class DependencyInjectionUtilitiesExtension
    {
        public static IServiceCollection AddUtilities(
            this IServiceCollection serviceCollection,
            IConfigurationSection emailConfigurationSection,
            IConfigurationSection encryptionConfigSection
        )
        {
            var result = bool.TryParse(emailConfigurationSection["EnableApiEmailCommunication"], out var condition);
            if (result && condition)
            {
                var settings = JsonSerializer.Deserialize<ApiEmailSettings>(
                    emailConfigurationSection["ApiCommunicationSettings"] ?? throw new InvalidOperationException()
                );
                serviceCollection.AddScoped<IEmailCommunicationUtility>(provider =>
                    new ApiEmailCommunicationUtility(settings ?? throw new InvalidOperationException())
                );
            }
            else
            {
                var settings = JsonSerializer.Deserialize<SmtpEmailSettings>(
                    emailConfigurationSection["SmtpCommunicationSettings"] ?? throw new InvalidOperationException()
                );
                serviceCollection.AddScoped<IEmailCommunicationUtility>(provider =>
                    new SmtpEmailCommunicationUtility(settings ?? throw new InvalidOperationException())
                );
            }

            var encryptionSettings = new EncryptionSettings
            {
                EncryptionKey = encryptionConfigSection["EncryptionKey"] ?? throw new InvalidOperationException(),
                Salt = Encoding.ASCII.GetBytes(encryptionConfigSection["Salt"] ?? throw new InvalidOperationException())
            };
            serviceCollection.AddSingleton(encryptionSettings);

            serviceCollection.AddScoped<IEncryptionHelper, DefaultEncryptionHelper>();

            return serviceCollection;
        }
    }
}