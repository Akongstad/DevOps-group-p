
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Prometheus;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
//For temp databases
//var tempFile = Path.GetTempFileName();

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



// csrf configuration
builder.Services.AddAntiforgery(options =>
{
    // Set Cookie properties using CookieBuilder properties†.
    options.FormFieldName = "AntiforgeryFieldname";
    options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
    options.SuppressXFrameOptionsHeader = false;
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//Prometheus
app.UseMetricServer();
app.UseHttpMetrics();
app.UseCors(MyAllowSpecificOrigins);
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
    
app.MapFallbackToFile("index.html");

//generate csrf tokens
app.UseAuthorization();
var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
app.Use((context, next) =>
{
    var requestPath = context.Request.Path.Value;

    if (string.Equals(requestPath, "/", StringComparison.OrdinalIgnoreCase)
        || string.Equals(requestPath, "/index.html", StringComparison.OrdinalIgnoreCase))
    {
        var tokenSet = antiforgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
            new CookieOptions { HttpOnly = false });
    }

    return next(context);
});

if (!app.Environment.IsEnvironment("Integration"))
{
    await app.SeedAsync();
}

app.Run();


public partial class Program { }
