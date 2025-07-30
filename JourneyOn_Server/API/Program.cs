using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("missing connection string");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddControllers();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    bool ok = await RdsDbConnection.ConnectToDbAsync(builder.Configuration,connectionString);
     Console.WriteLine(
         ok
             ? "✅ Connected to AWS RDS!"
             : "❌ Cannot connect to AWS RDS."
     );
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();