using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Enable controllers
builder.Services.AddControllers();

var app = builder.Build();

// Enable serving static files (like index.html)
app.UseDefaultFiles(); // serves index.html by default
app.UseStaticFiles();

// Enable routing to controllers
app.MapControllers();

app.Run();