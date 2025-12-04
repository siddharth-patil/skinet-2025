using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}); // Add your DbContext here

builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Add your repository here
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddCors();
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connString = builder.Configuration.GetConnectionString("Redis");
    if (string.IsNullOrEmpty(connString))
    {
        //throw new InvalidOperationException("Redis connection string is not configured.");
        throw new Exception("Cannot get redis connection string.");
    }
    var configuration = ConfigurationOptions.Parse(connString, true);
    return ConnectionMultiplexer.Connect(configuration); 
});

builder.Services.AddSingleton<ICartService,CartService>();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>().AddEntityFrameworkStores<StoreContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>(); //api/login api/register
app.MapHub<NotificationHub>("/hub/notifications");
app.MapFallbackToController("Index", "Fallback");

try
{
    using var scope = app.Services.CreateScope(); // Create a scope to get scoped services like DbContext
    var services = scope.ServiceProvider; // Get the service provider
    var context = services.GetRequiredService<StoreContext>(); // Get your DbContext
    await context.Database.MigrateAsync(); // Apply any pending migrations
    await StoreContextSeed.SeedAsync(context); // Seed the database
}
catch (Exception ex)
{
    Console.WriteLine(ex);
	throw;
}

app.Run();
