using NexoraBackend.Application.DTOs;

namespace NexoraBackend.API.GraphQL.Types;
// GraphQL mutation payloads always return data OR error — never throw exceptions to clients
public abstract class BasePayload<T> where T : class
{
    public T? Data { get; set; }
    public string? Error { get; set; }
    public bool Success => Data != null && Error == null;
}

public class AuthPayload : BasePayload<AuthResponseDto> { }
public class RefreshPayload : BasePayload<AuthResponseDto> { }
public class OrderPayload : BasePayload<OrderDto> { }
public class ProductPayload : BasePayload<ProductDto> { }
public class CartPayload : BasePayload<CartDto> { }
public class ReviewPayload : BasePayload<ReviewDto> { }
public class CouponPayload : BasePayload<CouponDto> { }
public class CategoryPayload : BasePayload<CategoryDto> { }
public class AppliedCouponPayload : BasePayload<AppliedCouponDto> { }
public class BooleanPayload
{
    public bool Data { get; set; }
    public string? Error { get; set; }
    public bool Success => Error == null;
}