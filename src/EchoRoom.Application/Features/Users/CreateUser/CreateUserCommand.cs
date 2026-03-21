namespace EchoRoom.Application.Features.Users.CreateUser;

public sealed class CreateUserCommand : IRequest<ApiResult<CreateUserResponse>>
{
    [Required, MaxLength(255)]
    public required string FullName { get; set; }

    [Required, EmailAddress, MaxLength(255)]
    public required string Email { get; set; }

    [Required, Phone, MaxLength(20)]
    public required string PhoneNumber { get; set; }

    [Required, MaxLength(50)]
    public required string StreamerNickname { get; set; }

    [Required, MinLength(6)]
    public required string Password { get; set; }

    [Required]
    public required DateTime BirthDate { get; set; }

    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    public bool StaySignIn { get; set; }
}