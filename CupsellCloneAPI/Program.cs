using CupsellCloneAPI.Core;
using CupsellCloneAPI.Database;
using CupsellCloneAPI.Database.BlobContainer;

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

builder.Services.AddServices();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseResponseCaching();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();