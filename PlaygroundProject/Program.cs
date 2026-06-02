using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SpeedyShop.Api.Data;
using SpeedyShop.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SpeedyShop API", Version = "v1" });
});

builder.Services.AddDbContext<SpeedyShopDbContext>(options =>
{
    var databaseProvider = builder.Configuration.GetValue<string>("Database:Provider") ?? "Sqlite";
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SpeedyShopSqlServer"));
    }
    else
    {
        var sqliteConnectionString = builder.Configuration.GetConnectionString("SpeedyShopSqlite")
            ?? "Data Source=App_Data/speedyshop.db";

        var dataSourcePrefix = "Data Source=";
        if (sqliteConnectionString.StartsWith(dataSourcePrefix, StringComparison.OrdinalIgnoreCase))
        {
            var databasePath = sqliteConnectionString[dataSourcePrefix.Length..].Trim();
            var directory = Path.GetDirectoryName(Path.GetFullPath(databasePath));
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
        }

        options.UseSqlite(sqliteConnectionString);
    }

    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddSingleton<IExternalCatalogClient, FakeExternalCatalogClient>();
builder.Services.AddScoped<IProductWorkshopService, ProductWorkshopService>();
builder.Services.AddScoped<IOrderWorkshopService, OrderWorkshopService>();
builder.Services.AddScoped<IReportWorkshopService, ReportWorkshopService>();

// IMemoryCache is registered for participants, but the expensive popular-products query intentionally does not use it yet.
builder.Services.AddMemoryCache();

var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"];

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("SpeedyShop.Api", serviceVersion: "1.0.0-workshop"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                // WORKSHOP: Performance Issue - DB tracing exists and reveals N+1/chatty queries, but is only partially tuned.
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
            })
            .AddHttpClientInstrumentation();

        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
        }
        else
        {
            tracing.AddConsoleExporter();
        }
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");

        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
        }
        else
        {
            metrics.AddConsoleExporter();
        }
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// WORKSHOP: Performance Issue - response compression is intentionally not enabled. Add Brotli/Gzip during the workshop.
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "SpeedyShop API" }))
    .WithName("Health");

if (app.Configuration.GetValue("Database:CreateOnStartup", true) || app.Configuration.GetValue("Seed:RunOnStartup", true))
{
    await DatabaseSeeder.SeedAsync(app.Services, app.Configuration);
}

app.Run();
