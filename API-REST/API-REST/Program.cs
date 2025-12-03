using Microsoft.EntityFrameworkCore;
using API_REST.Models; // tu namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbVentasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL")));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();