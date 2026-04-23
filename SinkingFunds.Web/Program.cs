using SinkingFunds.Application.Abstractions;
using SinkingFunds.Application.Services;
using SinkingFunds.Infrastructure.Adapters;
using SinkingFunds.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);
string connString = builder.Configuration.GetConnectionString("SinkingFundsDb");

// Add services to the container.

builder.Services.AddSingleton<IEnvelopeRepository>(_ => new SqliteEnvelopeRepository(connString));
builder.Services.AddScoped<EnvelopeService>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
SqliteSchemaInitializer initializerObj = new SqliteSchemaInitializer(connString);
initializerObj.SchemaVerification();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
