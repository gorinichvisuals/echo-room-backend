namespace EchoRoom.Database.Context.ModelsConfig;

internal sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(room => room.Id);

        builder.Property(room => room.Id)
            .ValueGeneratedNever();

        builder.Property(room => room.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(room => room.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(room => room.TokensForJoin)
            .IsRequired();

        builder.Property(room => room.MaxDurationMinutes)
            .IsRequired(false);

        builder.Property(room => room.CurrentConnections)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(room => room.TotalConnections)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(room => room.CreatedAt)
            .IsRequired();

        builder.Property(room => room.ClosedAt)
            .IsRequired(false);

        builder.HasOne(room => room.Creator)
            .WithMany(users => users.CreatedRooms)
            .HasForeignKey(room => room.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(room => room.Participants)
            .WithOne(roomParticipants => roomParticipants.Room)
            .HasForeignKey(roomParticipant => roomParticipant.RoomId);

        builder.HasIndex(room => room.IsActive);
        builder.HasIndex(room => room.CreatorId);
    }
}