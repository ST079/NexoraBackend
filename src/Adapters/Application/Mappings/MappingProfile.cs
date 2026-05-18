
using AutoMapper;
using NexoraBackend.Application.DTOs;
using NexoraBackend.Core.Domain;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Application.Mappings;
//all the mapping methods are inside the profile class
public class MappingProfile : Profile
{
    //syntax: CreateMap<Source, Destination>()
    //ForMember is used to specify how to map specific properties when the source and destination types don't match directly
    //eg: d.Price is decimal in DTO but Money in domain, so we map it to s.Price.Amount
    public MappingProfile()
    {
        //Product
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.CompareAtPrice, o => o.MapFrom(s => s.CompareAtPrice == null ? (decimal?)null : s.CompareAtPrice.Amount))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.Reviews.Count))
            .ForMember(d => d.ImageUrls, o => o.MapFrom(s => s.ImageUrls.ToList()));

        CreateMap<Product, ProductSummaryDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.FirstImageUrl, o => o.MapFrom(s => s.ImageUrls.FirstOrDefault()));

        //Order
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.Subtotal.Amount))
            .ForMember(d => d.TaxAmount, o => o.MapFrom(s => s.TaxAmount.Amount))
            .ForMember(d => d.ShippingCost, o => o.MapFrom(s => s.ShippingCost.Amount))
            .ForMember(d => d.DiscountAmount, o => o.MapFrom(s => s.DiscountAmount.Amount))
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Total.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Total.Currency))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.LineTotal.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.UnitPrice.Currency));


        //User
        CreateMap<User, UserDto>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Value))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));


        //Cart
        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Total.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Total.Currency));

        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.LineTotal.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.UnitPrice.Currency))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ProductImageUrl));


        //Address
        CreateMap<Address, AddressDto>();

        //Category
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => s.Slug.Value))
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent != null ? s.Parent.Name : null))
            .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count))
            .ForMember(d => d.Children, o => o.MapFrom(s => s.Children));

        CreateMap<Category, CategorySummaryDto>()
            .ForMember(d => d.Slug, o => o.MapFrom(s => s.Slug.Value))
            .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count));

        //  Review 
        CreateMap<Core.Domain.Review, ReviewDto>()
            .ForMember(d => d.Rating, o => o.MapFrom(s => s.Rating.Value))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User.FullName));

        CreateMap<Core.Domain.Review, ReviewSummaryDto>()
            .ForMember(d => d.Rating, o => o.MapFrom(s => s.Rating.Value))
            .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User.FullName));

        // Coupon
        CreateMap<Coupon, CouponDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.MaximumDiscountAmount, o =>
                o.MapFrom(s => s.MaximumDiscountAmount != null ? s.MaximumDiscountAmount.Amount : (decimal?)null))
            .ForMember(d => d.MaximumDiscountCurrency, o =>
                o.MapFrom(s => s.MaximumDiscountAmount != null ? s.MaximumDiscountAmount.Currency : null))
            .ForMember(d => d.MinimumOrderAmount, o =>
                o.MapFrom(s => s.MinimumOrderAmount != null ? s.MinimumOrderAmount.Amount : (decimal?)null))
            .ForMember(d => d.MinimumOrderCurrency, o =>
                o.MapFrom(s => s.MinimumOrderAmount != null ? s.MinimumOrderAmount.Currency : null));

        // Payment
        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Amount, o => o.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Amount.Currency))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}