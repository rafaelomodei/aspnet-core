using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.RespectBrowserAcceptHeader = true);
builder.Services.AddDbContext<PcGamerDb>(opt => opt.UseInMemoryDatabase("PcGamerLists"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();


app.MapGet("/", () => "Lista de PC GAMERS");

app.MapGet("/pcgamers", async (PcGamerDb db) =>
    await db.PcGamers.ToListAsync());

app.MapGet("/pcgamers/cpu={cpu}", async (string cpu, PcGamerDb db) =>
    await db.PcGamers.Where(pc => pc.CPU == cpu).ToListAsync());

app.MapGet("/pcgamers/ram={ram}", async (string ram, PcGamerDb db) =>
    await db.PcGamers.Where(pc => pc.Ram == ram).ToListAsync());

app.MapGet("/pcgamers/graphic={graphic}", async (string graphic, PcGamerDb db) =>
    await db.PcGamers.Where(pc => pc.graphic == graphic).ToListAsync());

app.MapGet("/pcgamer/id={id}", async (int id, PcGamerDb db) =>
    await db.PcGamers.FindAsync(id)
        is PcGamer pcGamer
            ? Results.Ok(pcGamer)
            : Results.NotFound());

app.MapPost("/pcgamer", async (PcGamer pcGamer, PcGamerDb db) =>
{
    db.PcGamers.Add(pcGamer);
    await db.SaveChangesAsync();

    return Results.Created($"/pcgamer/{pcGamer.Id}", pcGamer);
});

app.MapPut("/pcgamer/{id}", async (int id, PcGamer inputPcGamer, PcGamerDb db) =>
{
    var pcGamer = await db.PcGamers.FindAsync(id);

    if (pcGamer is null) return Results.NotFound();

    pcGamer.Name = inputPcGamer.Name;
    pcGamer.CPU = inputPcGamer.CPU;
    pcGamer.Ram = inputPcGamer.Ram;
    pcGamer.graphic = inputPcGamer.graphic;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/pcgamer/{id}", async (int id, PcGamerDb db) =>
{
    if (await db.PcGamers.FindAsync(id) is PcGamer pcGamer)
    {
        db.PcGamers.Remove(pcGamer);
        await db.SaveChangesAsync();
        return Results.Ok(pcGamer);
    }

    return Results.NotFound();
});

app.Run();

class PcGamer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CPU { get; set; }
    public string? Ram { get; set; }
    public string? graphic { get; set; }

}

class PcGamerDb : DbContext
{
    public PcGamerDb(DbContextOptions<PcGamerDb> options)
        : base(options) { }

    public DbSet<PcGamer> PcGamers => Set<PcGamer>();
}