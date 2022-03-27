using Microsoft.AspNetCore.Authentication.JwtBearer;
using MinitwitReact.Authentication;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
//Setup serilog
ConfigureLogging();
builder.Host.UseSerilog();


//------
builder.Services.AddControllersWithViews();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddDbContext<MinitwitContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Minitwit")));
builder.Services.AddScoped<IMinitwitContext, MinitwitContext>();
builder.Services.AddScoped<IMinitwit, Minitwit>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
//Cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("https://minitwit.online",
                "http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Secret").Value ?? "integration")),
        ValidateIssuer = false,
        ValidateAudience = false,
    });

var app = builder.Build();
//Prometheus
app.UseMetricServer();
app.UseHttpMetrics();
app.UseCors(myAllowSpecificOrigins);
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
    
app.MapFallbackToFile("index.html");

if (!app.Environment.IsEnvironment("Integration"))
{
    await app.SeedAsync();
}
app.Run();

void ConfigureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVORIONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
            optional: true)
        .Build();
    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment ?? "Development"))
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat =
            $"{configuration["ApplicationName"]}-logs-{environment.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
    };
}

//Used for integration testing
public partial class Program { }
