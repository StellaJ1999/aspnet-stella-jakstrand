using Domain.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MembershipOfferConfiguration : IEntityTypeConfiguration<MembershipOffer>
{
    public void Configure(EntityTypeBuilder<MembershipOffer> builder)
    {
        builder.ToTable("MembershipOffers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Tier).IsRequired();
        builder.Property(x => x.MonthlyPrice).IsRequired().HasColumnType("decimal(5,2)"); // maxpris på 999.99
        builder.Property(x => x.CreatedUtc).IsRequired();

        builder.HasIndex(x => x.Tier).IsUnique();
        builder.HasIndex(x => x.CreatedUtc);
    }
}
