using Microsoft.EntityFrameworkCore; // Essencial para DbContext, UseSqlServer
using APIMonitores; // Seu namespace raiz para AppDbContext
using APIMonitores.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o DbContext como um serviço
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração para Swagger/OpenAPI (geralmente para APIs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// POST: Criar monitor
app.MapPost("/monitores", async (AlunoMonitor monitor, AppDbContext db) =>
{
    db.Monitores.Add(monitor);
    await db.SaveChangesAsync();
    return Results.Created($"/monitores/{monitor.IdMonitor}", monitor);
});

// POST: Criar horário para monitor
app.MapPost("/horarios", async (Horario horario, AppDbContext db) =>
{
    db.Horarios.Add(horario);
    await db.SaveChangesAsync();
    return Results.Created($"/horarios/{horario.IdHorario}", horario);
});

// GET: Listar todos os monitores
app.MapGet("/monitores", async (AppDbContext db) =>
    await db.Monitores.ToListAsync());

// GET: Buscar monitor pelo apelido (com horários)
app.MapGet("/monitores/{apelido}", async (string apelido, AppDbContext db) =>
{
    var monitor = await db.Monitores
        .Include(m => m.Horarios)
        .FirstOrDefaultAsync(m => m.Apelido == apelido);

    return monitor is null ? Results.NotFound() : Results.Ok(monitor);
});

// PUT: Atualizar dados de um monitor
app.MapPut("/monitores/{id}", async (int id, AlunoMonitor updated, AppDbContext db) =>
{
    var monitor = await db.Monitores.FindAsync(id);
    if (monitor is null) return Results.NotFound();

    monitor.RA = updated.RA;
    monitor.Nome = updated.Nome;
    monitor.Apelido = updated.Apelido;
    await db.SaveChangesAsync();

    return Results.Ok(monitor);
});

// PUT: Atualizar horário de atendimento de um monitor
app.MapPut("/horarios/{id}", async (int id, Horario updated, AppDbContext db) =>
{
    var horario = await db.Horarios.FindAsync(id);
    if (horario is null) return Results.NotFound();

    horario.diaSemana = updated.diaSemana;
    horario.horario = updated.horario;
    await db.SaveChangesAsync();

    return Results.Ok(horario);
});


// DELETE: Excluir monitor (somente se não tiver horários)
app.MapDelete("/monitores/{id}", async (int id, AppDbContext db) =>
{
    var monitor = await db.Monitores
        .Include(m => m.Horarios)
        .FirstOrDefaultAsync(m => m.IdMonitor == id);

    if (monitor == null) return Results.NotFound();
    if (monitor.Horarios.Any()) return Results.BadRequest("Monitor possui horários cadastrados.");

    db.Monitores.Remove(monitor);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// DELETE: Excluir horário
app.MapDelete("/horarios/{id}", async (int id, AppDbContext db) =>
{
    var horario = await db.Horarios.FindAsync(id);
    if (horario is null) return Results.NotFound();

    db.Horarios.Remove(horario);
    await db.SaveChangesAsync();
    return Results.Ok();
});

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
//app.UseAuthorization();

// Mapear controladores (se usar controladores MVC/API)
//app.MapControllers(); // Se você tiver uma pasta Controllers

// Se você tiver endpoints mínimos definidos diretamente no Program.cs
// app.MapGet("/monitores", () => "Hello Monitores!");

app.Run();