using API.Utilities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register your application services
builder.Host.AddSerilogDocumentation(builder.Environment);
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance
            = $"{context.HttpContext.Request.Method}: {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.Add("requestID", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("missing connection string");
builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // you can tune these password/lockout settings as needed
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<IdentityApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});



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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages();
app.MapControllers();


await app.RunAsync();