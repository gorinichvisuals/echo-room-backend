namespace EchoRoom.Database.Context.ModelsConfig;

internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(country => country.Id);
        builder.HasIndex(country => country.Id);

        builder.Property(country => country.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(country => country.Name)
            .IsUnique();

        builder.Property(country => country.ISO3Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(country => country.ISO3Code)
            .IsUnique();

        builder.HasMany(country => country.Users)
            .WithOne(user => user.Country)
            .HasForeignKey(user => user.CountryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Country 
            { 
                Id = 1, 
                ISO3Code = "UKR",
                Name = "Ukraine" 
            },
            new Country
            {
                Id = 2,
                ISO3Code = "USA",
                Name = "United States of America"
            },
            new Country
            {
                Id = 3,
                ISO3Code = "FRA",
                Name = "France"
            });
    }
}