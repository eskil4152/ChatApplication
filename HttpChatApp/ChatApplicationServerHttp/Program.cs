using ChatApplicationServerHttp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5434;Database=eskil;Username=postgres;Password=password"));

builder.Services.AddScoped<DatabaseService>();

builder.Services.AddScoped<RoomActions>();
builder.Services.AddScoped<UserActions>();
builder.Services.AddScoped<WebSocketService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder.WithOrigins("https://localhost:3000")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowLocalhost");

app.UseWebSockets();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

