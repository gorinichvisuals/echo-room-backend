namespace EchoRoom.Database.Context.Models;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required string ISO3Code { get; set; }

    public ICollection<User> Users { get; set; } = [];
}