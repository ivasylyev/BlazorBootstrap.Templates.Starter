namespace NET9.BlazorWebAppServerGlobal.Models;

public record Customer2
(
    int CustomerId,
    string? CustomerName,
    string? Phone,
    string? Email,
    string? Address,
    string? PostalZip,
    string? Country
);
