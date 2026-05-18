using NexoraBackend.Application.DTOs;


namespace NexoraBackend.Adapters.API.GraphQL.Types;

public class ProductType : ObjectType<ProductDto>
{
    protected override void Configure(IObjectTypeDescriptor<ProductDto> descriptor)
    {
        descriptor.Description("A product available for purchase.");

        descriptor.Field(p => p.Id).Description("Unique product identifier.");
        descriptor.Field(p => p.Name).Description("Product name.");
        descriptor.Field(p => p.Price).Description("Current price.");
        descriptor.Field(p => p.StockQuantity).Description("Available stock units.");

        // Computed field: whether the product is in stock
        descriptor
            .Field("inStock")
            .Type<BooleanType>()
            .Resolve(ctx => ctx.Parent<ProductDto>().StockQuantity > 0);

        // Computed field: discount percentage
        descriptor
            .Field("discountPercentage")
            .Type<FloatType>()
            .Resolve(ctx =>
            {
                var product = ctx.Parent<ProductDto>();
                if (product.CompareAtPrice is null || product.CompareAtPrice <= product.Price)
                    return null;
                return Math.Round((product.CompareAtPrice.Value - product.Price) / product.CompareAtPrice.Value * 100, 2);
            });
    }
}