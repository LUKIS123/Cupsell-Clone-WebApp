using CupsellCloneAPI.Authentication;
using CupsellCloneAPI.Core;
using CupsellCloneAPI.Core.Utils;
using CupsellCloneAPI.Database;
using CupsellCloneAPI.Database.Authentication;
using CupsellCloneAPI.Database.BlobContainer;
using CupsellCloneAPI.Middleware;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlConnectionFactory(
    builder.Configuration.GetConnectionString("CupsellCloneSqlDbConnection") ?? throw new ArgumentNullException()
);
builder.Services.AddModelRepositories();

builder.Services.AddBlobStorage(
    builder.Configuration.GetConnectionString("AzureBLobConnectionString") ?? throw new ArgumentNullException(),
    builder.Configuration.GetSection("BlobContainerClient")["ContainerName"] ?? throw new ArgumentNullException()
);

builder.Services.AddCoreServices();
builder.Services.AddAutoMapper();

builder.Services.AddHttpContextAccessor();

// Auth
builder.Services.AddAuthenticationSettings(builder.Configuration.GetSection("Authentication"));
builder.Services.AddAuthServiceCollection();
builder.Services.AddTokenDatabaseRepositories();
builder.Services.AddAuthorizationHandlers();

// TODO: osobny DI extension do tego z Policy
builder.Services.AddAuthorization();


builder.Services.AddUtilities(builder.Configuration);
builder.Services.AddAccessors();

// NLog: Setup NLog for Dependency injection
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

// Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseResponseCaching();
app.UseStaticFiles();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();