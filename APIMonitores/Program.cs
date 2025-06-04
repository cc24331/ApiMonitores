using SuaAPI.Data;
using Microsoft.EntityFrameworkCore;
using SuaAPI.Models;
using Monitor = SuaAPI.Models.Monitor; // Necessário para acessar as classes Monitor e Horario

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicione suporte para serialização/deserialização de JSON
builder.Services.AddControllers();
// REMOVIDO: builder.Services.AddEndpointsApiExplorer();
// REMOVIDO: builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure o pipeline de requisições HTTP.
// REMOVIDO: if (app.Environment.IsDevelopment())
// REMOVIDO: {
// REMOVIDO:    app.UseSwagger();
// REMOVIDO:    app.UseSwaggerUI();
// REMOVIDO: }

// Endpoint de teste para buscar Monitores (já existente)
app.MapGet("/api/test/monitores", async (AppDbContext db) =>
{
    try
    {
        var monitores = await db.Monitores.Include(m => m.Horarios).ToListAsync(); // Incluir Horarios para ver o relacionamento
        return Results.Ok(monitores);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao acessar o banco de dados: {ex.Message}\nStackTrace: {ex.StackTrace}");
    }
});

// Endpoint para inserir um Monitor
app.MapPost("/api/monitores", async (Monitor novoMonitor, AppDbContext db) =>
{
    try
    {
        // Certifique-se de que o IdMonitor não está sendo enviado ou está 0 para que seja gerado automaticamente
        if (novoMonitor.IdMonitor != 0)
        {
            return Results.BadRequest("IdMonitor não deve ser fornecido para uma nova criação.");
        }

        // Se houver horários, garanta que o IdHorario deles também seja 0 para nova criação
        if (novoMonitor.Horarios != null)
        {
            foreach (var horario in novoMonitor.Horarios)
            {
                if (horario.IdHorario != 0)
                {
                    return Results.BadRequest("IdHorario não deve ser fornecido para um novo horário em conjunto com o monitor.");
                }
            }
        }

        db.Monitores.Add(novoMonitor);
        await db.SaveChangesAsync(); // Salva o novo monitor e seus horários (se houver)

        // Retorna o monitor criado com o Id gerado pelo banco de dados
        return Results.Created($"/api/monitores/{novoMonitor.IdMonitor}", novoMonitor);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao inserir Monitor: {ex.Message}\nStackTrace: {ex.StackTrace}");
    }
});

// Endpoint para inserir um Horário em um Monitor existente
app.MapPost("/api/monitores/{monitorId}/horarios", async (int monitorId, Horario novoHorario, AppDbContext db) =>
{
    try
    {
        // Verifica se o monitor existe
        var monitorExistente = await db.Monitores.FindAsync(monitorId);
        if (monitorExistente == null)
        {
            return Results.NotFound($"Monitor com Id {monitorId} não encontrado.");
        }

        // Garante que o IdHorario não está sendo enviado ou está 0
        if (novoHorario.IdHorario != 0)
        {
            return Results.BadRequest("IdHorario não deve ser fornecido para uma nova criação.");
        }

        // Associa o horário ao monitor
        novoHorario.IdMonitor = monitorId;
        novoHorario.Monitor = monitorExistente; // Opcional, o EF Core já cuidaria disso com IdMonitor

        db.Horarios.Add(novoHorario);
        await db.SaveChangesAsync();

        return Results.Created($"/api/monitores/{monitorId}/horarios/{novoHorario.IdHorario}", novoHorario);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro ao inserir Horário: {ex.Message}\nStackTrace: {ex.StackTrace}");
    }
});

app.MapControllers();

app.Run();