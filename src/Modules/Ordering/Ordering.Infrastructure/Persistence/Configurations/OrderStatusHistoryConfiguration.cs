using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistories");

        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).UseIdentityByDefaultColumn();
        builder.Property(history => history.IsOrderCreation).HasDefaultValue(false);
        builder.HasIndex(history => history.OrderId);
    }
}
