using IsekaiFantasyBE.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner
builder.Services.AddControllers();

// Adicionar o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configuração adicional (opcional), como título e versão
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Isekai Fantasy backend API",
        Version = "v0.1.0",
        Description = "API de backend service do Isekai Fantasy",
    });
});

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configurar o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API MySQL com .NET v1");
        c.RoutePrefix = string.Empty;  // Configura a UI do Swagger para a raiz da aplicação
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();