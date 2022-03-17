using Microsoft.AspNetCore.Authentication.JwtBearer;
using MinitwitReact.Authentication;
using Prometheus;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);


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
    options.AddPolicy(name: MyAllowSpecificOrigins,
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("https://minitwit.online",
                "http://localhost:3000", "*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var secret = builder.Configuration.GetSection("AppSettings:Secret");
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
app.UseCors(MyAllowSpecificOrigins);
app.UseStaticFiles();
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


public partial class Program { }
