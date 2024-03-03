using CupsellCloneAPI.Authentication;
using CupsellCloneAPI.Core;
using CupsellCloneAPI.Core.Utils;
using CupsellCloneAPI.Database;
using CupsellCloneAPI.Database.Authentication;
using CupsellCloneAPI.Database.BlobContainer;
using CupsellCloneAPI.EmailCommunication;
using CupsellCloneAPI.Middleware;
using CupsellCloneAPI.Validators;
using FluentValidation.AspNetCore;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlConnectionFactory(
    builder.Configuration.GetConnectionString("CupsellCloneSqlDbConnection") ??
    throw new ArgumentNullException(string.Empty, "SQL Connection string is empty")
);
builder.Services.AddModelRepositories();

builder.Services.AddBlobStorage(
    builder.Configuration.GetConnectionString("AzureBLobConnectionString") ??
    throw new ArgumentNullException(string.Empty, "Blob connection string is empty"),
    builder.Configuration.GetSection("BlobContainerClient")["ContainerName"] ??
    throw new ArgumentNullException(string.Empty, "Blob container client is empty")
);

builder.Services.AddCoreServices();
builder.Services.AddAutoMapper();

builder.Services.AddHttpContextAccessor();

// Auth
builder.Services.AddAuthenticationSettings(builder.Configuration.GetSection("Authentication"));
builder.Services.AddAuthServiceCollection();
builder.Services.AddTokenDatabaseRepositories();
builder.Services.AddAuthorizationHandlers();

builder.Services.AddAuthorization();


builder.Services.AddUtilities(builder.Configuration);
builder.Services.AddAccessors();
builder.Services.AddEmailCommunication(builder.Configuration);

// NLog: Setup NLog for Dependency injection
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

// Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<ApiEndpointMiddleware>();

// Fluent Validation
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidators();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("FrontEndClient", corsPolicyBuilder =>
    {
        var origins = builder.Configuration
            .GetSection("AllowedOrigins")
            .GetChildren()
            .Select(x => x.Value)
            .Where(x => x is not null)
            .ToArray();

        corsPolicyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Location")
            .WithOrigins(origins!);
    });
});

var app = builder.Build();

app.UseResponseCaching();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(_ => { });

// Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseMiddleware<ApiEndpointMiddleware>();

app.UseSpa(x =>
{
    x.UseProxyToSpaDevelopmentServer("https://localhost:5173");
    x.Options.SourcePath = "CupsellCloneClient";
});

app.MapFallbackToFile("/index.html");

app.Run();


// TODO: napisac tabelki dla zamowien, byc moze dla koszyka tez lub trzymac koszyk w sesji