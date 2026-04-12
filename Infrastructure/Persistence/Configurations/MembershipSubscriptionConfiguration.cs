using Domain.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MembershipSubscriptionConfiguration : IEntityTypeConfiguration<MembershipSubscription>
{
    public void Configure(EntityTypeBuilder<MembershipSubscription> builder)
    {
        builder.ToTable("MembershipSubscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        builder.Property(x => x.MembershipOfferId).IsRequired();

        builder.Property(x => x.DurationInMonths).IsRequired();
        builder.Property(x => x.StartUtc).IsRequired();
        builder.Property(x => x.EndUtc).IsRequired();

        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedUtc).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => x.EndUtc);
    }
}
