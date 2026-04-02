namespace EchoRoom.Database.Context.ModelsConfig;

internal sealed class RoomParticipantConfiguration : IEntityTypeConfiguration<RoomParticipant>
{
    public void Configure(EntityTypeBuilder<RoomParticipant> builder)
    {
        builder.HasKey(roomParticipant => roomParticipant.Id);

        builder.Property(roomParticipant => roomParticipant.Id)
            .ValueGeneratedNever();

        builder.Property(roomParticipant => roomParticipant.JoinedAt)
            .IsRequired();

        builder.Property(roomParticipant => roomParticipant.LeftAt)
            .IsRequired(false);

        builder.Property(roomParticipant => roomParticipant.ConnectionId)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasOne(roomParticipant => roomParticipant.Room)
            .WithMany(rooms => rooms.Participants)
            .HasForeignKey(roomParticipant => roomParticipant.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(roomParticipant => roomParticipant.User)
            .WithMany(users => users.RoomParticipants)
            .HasForeignKey(roomParticipant => roomParticipant.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(roomParticipant => roomParticipant.RoomId);
        builder.HasIndex(roomParticipant => roomParticipant.UserId);
    }
}