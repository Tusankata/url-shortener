using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Application.Entities;

namespace UrlShortener.Application.Infrastructure.Persistence.Configurations

{
    public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
    {
        public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
        {
            builder.ToTable("ShortenedUrls");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.ShortCode).IsRequired();
            builder.Property(x => x.ShortUrl).IsRequired();
            builder.Property(x => x.LongUrl).IsRequired();
        }
    }
}