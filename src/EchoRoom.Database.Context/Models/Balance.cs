namespace EchoRoom.Database.Context.Models;

public sealed class Balance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }

    public User User { get; set; } = null!;
}