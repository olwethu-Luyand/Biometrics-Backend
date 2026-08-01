using BiometricClockingAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => Results.Content(@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <title>Biometric Clocking API</title>
</head>
<body>
    <h1>Biometric Clocking API</h1>
    <p>The backend is running. Use the API endpoints below:</p>
    <ul>
        <li><a href=""/api/Employee"">GET /api/Employee</a></li>
        <li><code>POST /api/Employee</code></li>
        <li><code>PUT /api/Employee/{id}</code></li>
        <li><code>DELETE /api/Employee/{id}</code></li>
    </ul>
</body>
</html>", "text/html"));

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
