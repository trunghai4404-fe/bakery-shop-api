using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public static class OrderingDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        bool hasChanges = false;

        // 1. Seed Payment Method (Chỉ COD)
        if (!await context.PaymentMethods.AnyAsync())
        {
            await context.PaymentMethods.AddAsync(new PaymentMethod(
                1,
                "COD",
                "Thanh toán khi nhận hàng",
                "Nhận bánh kiểm tra rồi thanh toán tiền mặt",
                null,
                true,
                1
            ));
            hasChanges = true;
        }

        // 2. Seed Shipping Method (Chỉ INTERNAL)
        if (!await context.ShippingMethods.AnyAsync())
        {
            await context.ShippingMethods.AddAsync(new ShippingMethod(
                1,
                "INTERNAL",
                "Giao hàng nội bộ",
                0,
                "Phí ship sẽ được tính khi giao hàng",
                "30-60 phút",
                true,
                1
            ));
            hasChanges = true;
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }
    }
}
