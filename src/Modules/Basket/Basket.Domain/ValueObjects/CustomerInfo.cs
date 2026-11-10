namespace Basket.Domain.ValueObjects;

public record CustomerInfo(
    string Name,
    string Phone,
    string Email
) 
{
    public static CustomerInfo Empty => new CustomerInfo(string.Empty, string.Empty, string.Empty);
}
