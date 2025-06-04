using Microsoft.EntityFrameworkCore;
using APIMonitores.Data;   // << CORRIGIDO PARA O NAMESPACE CORRETO
using APIMonitores.Models; // << CORRIGIDO PARA O NAMESPACE CORRETO
using System.Linq;
using APIMonitores.Data;
using Microsoft.EntityFrameworkCore;
using APIMonitores.Data;   // << VERIFIQUE ESTE NAMESPACE (deve bater com o do AppDbContext)
using APIMonitores.Models; // << VERIFIQUE ESTE NAMESPACE (deve bater com o dos modelos)
using System.Linq;

// ... (restante do código)

var builder = WebApplication.CreateBuilder(args);


// --- Configuração de Serviços ---
// Adiciona o suporte a Swagger/OpenAPI para documentação da API e interface de teste
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o DbContext para usar SQL Server
// A connection string será lida do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- Configuração do Pipeline de Requisições HTTP ---
// No ambiente de desenvolvimento, habilita o Swagger UI para testar a API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redireciona requisições HTTP para HTTPS

// --- Definição dos Endpoints da API ---

// POST: Incluir Monitor
app.MapPost("/monitores", async (Monitor monitor, AppDbContext db) =>
{
    db.Monitores.Add(monitor);
    await db.SaveChangesAsync();
    return Results.Created($"/monitores/{monitor.IdMonitor}", monitor); // Retorna 201 Created com a URL do novo recurso
});

// POST: Incluir horário do monitor
app.MapPost("/horarios", async (Horario horario, AppDbContext db) =>
{
    // Verifica se o monitor ao qual o horário será associado existe
    var monitorExiste = await db.Monitores.AnyAsync(m => m.IdMonitor == horario.IdMonitor);
    if (!monitorExiste)
    {
        return Results.NotFound($"Monitor com Id {horario.IdMonitor} não encontrado."); // Retorna 404 Not Found
    }

    db.Horarios.Add(horario);
    await db.SaveChangesAsync();
    return Results.Created($"/horarios/{horario.IdHorario}", horario); // Retorna 201 Created
});

// GET: Listar todos os monitores
app.MapGet("/monitores", async (AppDbContext db) =>
{
    return Results.Ok(await db.Monitores.ToListAsync()); // Retorna 200 OK com a lista de monitores
});

// GET: Selecionar um monitor específico pelo apelido, retornando seus dados e horários
app.MapGet("/monitores/{apelido}", async (string apelido, AppDbContext db) =>
{
    var monitor = await db.Monitores
                          .Include(m => m.Horarios) // Carrega os horários relacionados ao monitor
                          .FirstOrDefaultAsync(m => m.Apelido == apelido);

    if (monitor == null)
    {
        return Results.NotFound($"Monitor com apelido '{apelido}' não encontrado."); // Retorna 404 Not Found
    }

    // Retorna os dados do monitor e uma projeção simples dos horários
    return Results.Ok(new
    {
        monitor.IdMonitor,
        monitor.RA,
        monitor.Nome,
        monitor.Apelido,
        Horarios = monitor.Horarios.Select(h => new { h.DiaSemana, h.HorarioTexto })
    });
});

// PUT: Alterar monitor
app.MapPut("/monitores/{id}", async (int id, Monitor updatedMonitor, AppDbContext db) =>
{
    if (id != updatedMonitor.IdMonitor)
    {
        return Results.BadRequest("ID do monitor na URL não corresponde ao ID no corpo da requisição."); // Retorna 400 Bad Request
    }

    var existingMonitor = await db.Monitores.FindAsync(id);
    if (existingMonitor == null)
    {
        return Results.NotFound($"Monitor com Id {id} não encontrado."); // Retorna 404 Not Found
    }

    // Atualiza as propriedades do monitor existente
    existingMonitor.RA = updatedMonitor.RA;
    existingMonitor.Nome = updatedMonitor.Nome;
    existingMonitor.Apelido = updatedMonitor.Apelido;

    await db.SaveChangesAsync();
    return Results.NoContent(); // Retorna 204 No Content (indica sucesso sem conteúdo para retornar)
});

// PUT: Alterar horário do monitor em determinado dia da semana
app.MapPut("/horarios/{idHorario}", async (int idHorario, Horario updatedHorario, AppDbContext db) =>
{
    if (idHorario != updatedHorario.IdHorario)
    {
        return Results.BadRequest("ID do horário na URL não corresponde ao ID no corpo da requisição."); // Retorna 400 Bad Request
    }

    var existingHorario = await db.Horarios.FindAsync(idHorario);
    if (existingHorario == null)
    {
        return Results.NotFound($"Horário com Id {idHorario} não encontrado."); // Retorna 404 Not Found
    }

    // Opcional: Se a FK IdMonitor for alterada, verifica se o novo monitor existe.
    if (existingHorario.IdMonitor != updatedHorario.IdMonitor)
    {
        var monitorExiste = await db.Monitores.AnyAsync(m => m.IdMonitor == updatedHorario.IdMonitor);
        if (!monitorExiste)
        {
            return Results.BadRequest($"Monitor com Id {updatedHorario.IdMonitor} não encontrado para associar ao horário.");
        }
    }

    // Atualiza as propriedades do horário existente
    existingHorario.DiaSemana = updatedHorario.DiaSemana;
    existingHorario.HorarioTexto = updatedHorario.HorarioTexto;
    existingHorario.IdMonitor = updatedHorario.IdMonitor; // Garante que a FK também seja atualizada

    await db.SaveChangesAsync();
    return Results.NoContent(); // Retorna 204 No Content
});

// DELETE: Excluir monitor se não houver referência de horário
app.MapDelete("/monitores/{id}", async (int id, AppDbContext db) =>
{
    // Inclui os horários para verificar se há dependências
    var monitor = await db.Monitores.Include(m => m.Horarios).FirstOrDefaultAsync(m => m.IdMonitor == id);

    if (monitor == null)
    {
        return Results.NotFound($"Monitor com Id {id} não encontrado."); // Retorna 404 Not Found
    }

    // Verifica se existem horários associados antes de excluir
    if (monitor.Horarios.Any())
    {
        return Results.Conflict($"Não é possível excluir o monitor com Id {id} pois existem horários associados a ele."); // Retorna 409 Conflict
    }

    db.Monitores.Remove(monitor);
    await db.SaveChangesAsync();
    return Results.NoContent(); // Retorna 204 No Content
});

// DELETE: Excluir horário
app.MapDelete("/horarios/{id}", async (int id, AppDbContext db) =>
{
    var horario = await db.Horarios.FindAsync(id);

    if (horario == null)
    {
        return Results.NotFound($"Horário com Id {id} não encontrado."); // Retorna 404 Not Found
    }

    db.Horarios.Remove(horario);
    await db.SaveChangesAsync();
    return Results.NoContent(); // Retorna 204 No Content
});

app.Run(); // Inicia a aplicação