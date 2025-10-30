using System.Text.Json.Serialization;
using AppBase.Config.Data;
using AppBase.Config.Extensions;
using AppBase.Config.Filters;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO.Converters;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => { options.Filters.Add<GlobalExceptionFilter>(); })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    })
    .AddDataAnnotationsLocalization();
builder.Services.AddApplicationServices();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DB_CONNECTION");
var evConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
if (evConnectionString != null) connectionString = evConnectionString;

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        x => x.UseNetTopologySuite()
    ));

var app = builder.Build();

// Default metrics
app.UseHttpMetrics();
app.MapMetrics();
// HTTP standard metrics 
app.UseHttpMetrics(options =>
{
    // Enabled metrics
    options.RequestCount.Enabled = true;
    options.RequestDuration.Enabled = true;
    options.InProgress.Enabled = true;
});
//System metrics
app.UseMetricServer();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API");
        options.RoutePrefix = "docs";
        options.EnableTryItOutByDefault();
    });
}

// Run database migrations automatically
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApiDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseAuthorization();
app.MapControllers();
app.UseRouting();
app.UseSelectiveMetrics();

app.Map("/", () => "API WORKING!!!");

app.Run();