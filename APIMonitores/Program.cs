using Microsoft.EntityFrameworkCore; // Essencial para DbContext, UseSqlServer
using APIMonitores; // Seu namespace raiz para AppDbContext

var builder = WebApplication.CreateBuilder(args);

// Adicionar o DbContext como um serviço
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração para Swagger/OpenAPI (geralmente para APIs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline de requisições HTTP

// Habilitar Swagger UI em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirecionamento HTTPS (boa prática)
app.UseHttpsRedirection();

// Adicionar autorização (se sua API usar)
app.UseAuthorization();

// Mapear controladores (se usar controladores MVC/API)
app.MapControllers(); // Se você tiver uma pasta Controllers

// Se você tiver endpoints mínimos definidos diretamente no Program.cs
// app.MapGet("/monitores", () => "Hello Monitores!");

app.Run();