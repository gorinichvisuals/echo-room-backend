namespace EchoRoom.Database.Context.Models;

public class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int CountryId { get; set; }
    public required string FullName { get; set; }
    public required string StreamerNickname { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public DateTime BirthDate { get; set; }

    public bool IsStreamer { get; set; }
    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }

    public User()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        LastLoginAt = DateTime.UtcNow;
    }

    public Role Role { get; set; } = null!;
    public Country Country { get; set; } = null!;
}