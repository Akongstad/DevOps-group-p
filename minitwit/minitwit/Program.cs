var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapFallbackToFile("./../index.html");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.Run(); 

