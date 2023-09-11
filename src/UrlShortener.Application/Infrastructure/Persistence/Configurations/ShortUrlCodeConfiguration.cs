using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Application.Entities;
using UrlShortener.Application.ValueObjects;

namespace UrlShortener.Application.Infrastructure.Persistence.Configurations;

public class ShortUrlCodeConfiguration : IEntityTypeConfiguration<ShortUrlCode>
{
    public void Configure(EntityTypeBuilder<ShortUrlCode> builder)
    {
        builder.ToTable("ShortUrlCodes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.State).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.Property(x => x.Code)
            .HasConversion(
                code => code.Value,
                value => Code.Create(value))
            .IsRequired();
    }
}
