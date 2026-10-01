using GranDT_ET12.Repositorios;
using GranDT_ET12.Repositorios.interfaces;
using GranDT_ET12.Service;
using GranDT_ET12.Service.interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Repositorios
// "x => new ...(cadena)" significa: cuando alguien pida esta interfaz, se crea este repositorio con la cadena de conexion
string cadena = BdConexion.ObtenerConnectionString();
builder.Services.AddScoped<IUsuarioRepositorio>(x => new UsuarioRepositorio(cadena));
builder.Services.AddScoped<IEquipoRepositorio>(x => new EquipoRepositorio(cadena));
builder.Services.AddScoped<IFutbolistaRepositorio>(x => new FutbolistaRepositorio(cadena));
builder.Services.AddScoped<IPlantillaRepositorio>(x => new PlantillaRepositorio(cadena));
builder.Services.AddScoped<IDetallePlantillaRepositorio>(x => new DetallePlantillaRepositorio(cadena));
builder.Services.AddScoped<IPuntuacionRepositorio>(x => new PuntuacionRepositorio(cadena));

// Servicios (logica de negocio)
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IFutbolistaService, FutbolistaService>();
builder.Services.AddScoped<IPuntuacionService, PuntuacionService>();
builder.Services.AddScoped<IPlantillaService, PlantillaService>();

var app = builder.Build();

// Si ocurre un error que no controlamos, se responde 500 en formato JSON (ver ErrorController)
app.UseExceptionHandler("/error");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // documentacion de la API en /swagger
}

app.MapControllers();

app.Run();
