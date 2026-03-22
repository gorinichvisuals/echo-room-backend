namespace EchoRoom.Database.Context.ModelsConfig;

internal sealed class BalanceConfiguration : IEntityTypeConfiguration<Balance>
{
    public void Configure(EntityTypeBuilder<Balance> builder)
    {
        builder.HasKey(balance => balance.Id);
        builder.HasIndex(balance => balance.Id);

        builder.Property(balance => balance.Amount)
            .HasPrecision(8, 2);

        builder.Property(balance => balance.Currency)
            .HasConversion<string>();

        builder.HasOne(balance => balance.User)
            .WithMany(user => user.Balances)
            .HasForeignKey(balance => balance.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}