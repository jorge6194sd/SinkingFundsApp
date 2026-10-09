using SinkingFunds.Application.Abstractions;
using SinkingFunds.Application.Services;
using SinkingFunds.Infrastructure.Adapters;
using SinkingFunds.Infrastructure.Persistence;
using SinkingFunds.Web.Middleware; // ADDED

var builder = WebApplication.CreateBuilder(args);

string connString = builder.Configuration.GetConnectionString("SinkingFundsDb")
    ?? throw new InvalidOperationException(
        "Missing connection string 'SinkingFundsDb'.");

// Add services to the container.
builder.Services.AddSingleton<IEnvelopeRepository>(
    _ => new SqliteEnvelopeRepository(connString));

builder.Services.AddScoped<EnvelopeService>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddOpenApi();

var app = builder.Build();

SqliteSchemaInitializer initializerObj = new SqliteSchemaInitializer(connString);
initializerObj.SchemaVerification();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ADDED: translate DomainValidationException into a 400 response.
app.UseMiddleware<ApiExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseStaticFiles();

app.Run();