namespace NexoraBackend.API.GraphQL.Types;

// Auth
public record RegisterInput(string FirstName, string LastName, string Email, string Password);
public record LoginInput(string Email, string Password);

// Order
public record PlaceOrderInput(
    string Line1, string? Line2, string City,
    string State, string PostalCode, string Country,
    string? CouponCode = null,
    string Currency = "USD");

// Cart
public record AddToCartInput(Guid ProductId, int Quantity);
public record UpdateCartItemInput(Guid ProductId, int Quantity);

// Product (admin)
public record CreateProductInput(
    string Name, string Description, decimal Price, string Currency,
    int StockQuantity, Guid CategoryId,
    bool IsFeatured = false, List<string>? ImageUrls = null);

public record UpdateProductInput(
    Guid Id, string? Name, string? Description,
    decimal? Price, string? Currency, bool? IsActive, bool? IsFeatured);

public record UpdateStockInput(Guid ProductId, int Quantity);

// Category (admin)
public record CreateCategoryInput(
    string Name, string Description, Guid? ParentId,
    string? ImageUrl, int SortOrder = 0);

public record UpdateCategoryInput(
    Guid Id, string Name, string Description,
    string? ImageUrl, int SortOrder);

// Review
public record SubmitReviewInput(Guid ProductId, int Rating, string Title, string Body);
public record ModerateReviewInput(Guid ReviewId, bool Approve, string? RejectionReason = null);

// Coupon (admin)
public record CreateCouponInput(
    string Code, string Description, string Type, decimal DiscountValue,
    DateTime? ExpiresAt = null, int? MaxUsageCount = null,
    int? MaxUsagePerUser = null, decimal? MinimumOrderAmount = null,
    string Currency = "USD");