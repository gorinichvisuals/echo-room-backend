namespace EchoRoom.Application.Features.Users.GetPersonalInfo;

public sealed class UserGetPersonalInfoResponse
{
    public int Id { get; set; }
    public required string StreamerNickname { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Age => BirthDate.CalculateAge();

    public RoleDto Role { get; set; } = null!;
    public CountryDto Country { get; set; } = null!;
    public ICollection<BalanceDto> Balances { get; set; } = [];
}

public class RoleDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class CountryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class BalanceDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
}