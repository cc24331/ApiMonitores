using Microsoft.EntityFrameworkCore; 
using APIMonitores; 
using APIMonitores.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/monitores", async (AlunoMonitor monitor, AppDbContext db) =>
{
    db.Monitores.Add(monitor);
    await db.SaveChangesAsync();
    return Results.Created($"/monitores/{monitor.IdMonitor}", monitor);
});

app.MapPost("/horarios", async (Horario horario, AppDbContext db) =>
{
    db.Horarios.Add(horario);
    await db.SaveChangesAsync();
    return Results.Created($"/horarios/{horario.IdHorario}", horario);
});

app.MapGet("/monitores", async (AppDbContext db) =>
    await db.Monitores.ToListAsync());

app.MapGet("/monitores/{apelido}", async (string apelido, AppDbContext db) =>
{
    var monitor = await db.Monitores
        .Include(m => m.Horarios)
        .FirstOrDefaultAsync(m => m.Apelido == apelido);

    return monitor is null ? Results.NotFound() : Results.Ok(monitor);
});

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

app.MapPut("/horarios/{id}", async (int id, Horario updated, AppDbContext db) =>
{
    var horario = await db.Horarios.FindAsync(id);
    if (horario is null) return Results.NotFound();

    horario.diaSemana = updated.diaSemana;
    horario.horario = updated.horario;
    await db.SaveChangesAsync();

    return Results.Ok(horario);
});

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

app.Run();