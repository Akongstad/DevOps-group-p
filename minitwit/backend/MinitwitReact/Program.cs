using Microsoft.AspNetCore.Authentication.JwtBearer;
using MinitwitReact.Authentication;
using Prometheus;

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

 
//------
builder.Services.AddControllersWithViews();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddDbContext<MiniTwitContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Minitwit")));
builder.Services.AddScoped<IMiniTwitContext, MiniTwitContext>();
builder.Services.AddScoped<IMinitwit, Minitwit>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");
    
app.MapFallbackToFile("index.html");

if (!app.Environment.IsEnvironment("Integration"))
{
    await app.SeedAsync();
}

app.Run();
//Used for integration testing
public partial class Program { }
