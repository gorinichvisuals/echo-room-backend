namespace EchoRoom.Database.Context.ModelsConfig;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.HasMany(role => role.Users)
            .WithOne(users => users.Role)
            .HasForeignKey(users => users.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Role 
            { 
                Id = 1, 
                Name = "Admin" 
            },
            new Role 
            { 
                Id = 2, 
                Name = "Streamer" 
            },
            new Role 
            { 
                Id = 3, 
                Name = "BaseUser" 
            });
    }
}