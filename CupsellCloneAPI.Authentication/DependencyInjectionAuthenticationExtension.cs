using System.Text;
using CupsellCloneAPI.Authentication.Authenticators;
using CupsellCloneAPI.Authentication.Authorization;
using CupsellCloneAPI.Authentication.EmailAuthenticationHelper;
using CupsellCloneAPI.Authentication.Services;
using CupsellCloneAPI.Authentication.Settings;
using CupsellCloneAPI.Authentication.TokenGenerators;
using CupsellCloneAPI.Authentication.TokenValidators;
using CupsellCloneAPI.Database.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CupsellCloneAPI.Authentication
{
    public static class DependencyInjectionAuthenticationExtension
    {
        public static IServiceCollection AddAuthenticationSettings(
            this IServiceCollection serviceCollection,
            IConfigurationSection configSection
        )
        {
            var authenticationSettings = new AuthenticationSettings();
            configSection.Bind(authenticationSettings);
            serviceCollection.AddSingleton(authenticationSettings);

            serviceCollection.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = "Bearer";
                opt.DefaultScheme = "Bearer";
                opt.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            return serviceCollection;
        }

        public static IServiceCollection AddAuthServiceCollection(this IServiceCollection serviceCollection)
        {
            // Password hasher
            serviceCollection.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            // Services
            serviceCollection.AddScoped<ITokenGenerator, DefaultTokenGenerator>();
            serviceCollection.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
            serviceCollection.AddScoped<IAuthenticator, Authenticator>();
            serviceCollection.AddScoped<IAccountService, AccountService>();
            serviceCollection.AddScoped<IEmailCommunicationHelper, EmailCommunicationHelper>();
            return serviceCollection;
        }

        public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            return serviceCollection;
        }
    }
}