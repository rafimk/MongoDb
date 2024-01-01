using MongoDb.Dtos;
using MongoDb.Entities;
using MongoDb.MongoDb;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMongo(builder.Configuration);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/item", async (IRepository<Item> repository) =>
    {
        return (await repository.GetAllAsync()).Select(x => x.AsDto());
    })
.WithName("GetAsync")
.WithOpenApi();

app.MapGet("/item/{id}", async (IRepository<Item> repository, Guid id) =>
    {
        var item = await repository.GetAsync(id);

        return item == null ? Results.NotFound() : Results.Ok(item.AsDto());
    })
.WithName("GetByIdAsync")
.WithOpenApi();

app.MapPost("/item", async (IRepository<Item> repository, CreateItemCommand command) =>
    {
        var item = new Item
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await repository.CreateAsync(item);

        var key = item.Id;

        return Results.Created();
    })
.WithName("CreateItem")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
