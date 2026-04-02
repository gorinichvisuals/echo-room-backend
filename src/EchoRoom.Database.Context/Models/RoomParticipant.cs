namespace EchoRoom.Database.Context.Models;

public sealed class RoomParticipant
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public int UserId { get; set; }
    public string? ConnectionId { get; set; }

    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }

    public RoomParticipant()
    {
        Id = Guid.NewGuid();
        JoinedAt = DateTime.UtcNow;
    }

    public Room Room { get; set; } = null!;
    public User User { get; set; } = null!;
}