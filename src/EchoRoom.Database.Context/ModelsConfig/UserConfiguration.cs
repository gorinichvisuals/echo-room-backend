namespace EchoRoom.Database.Context.ModelsConfig;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.HasIndex(user => user.Id);

        builder.Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(user => user.StreamerNickname)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(user => user.StreamerNickname)
            .IsUnique();

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(user => user.PhoneNumber)
            .IsUnique();

        builder.Property(user => user.Password)
            .IsRequired();

        builder.Property(user => user.BirthDate)
            .IsRequired();

        builder.Property(user => user.IsStreamer)
            .HasDefaultValue(false);

        builder.Property(user => user.IsAdmin)
            .HasDefaultValue(false);

        builder.HasOne(user => user.Role)
            .WithMany(role => role.Users)
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(user => user.Country)
            .WithMany(country => country.Users)
            .HasForeignKey(user => user.CountryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}