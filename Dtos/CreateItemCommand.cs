namespace MongoDb.Dtos;

public record CreateItemCommand(string Name, string Description, decimal Price);
