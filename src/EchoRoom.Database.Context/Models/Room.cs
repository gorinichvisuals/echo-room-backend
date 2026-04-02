namespace EchoRoom.Database.Context.Models;

public sealed class Room
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public int CreatorId { get; set; }
    public bool IsActive { get; set; }
    public int TokensForJoin { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public int CurrentConnections { get; set; }
    public int TotalConnections { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Room()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public User Creator { get; set; } = null!;
    public ICollection<RoomParticipant> Participants { get; set; } = [];
}