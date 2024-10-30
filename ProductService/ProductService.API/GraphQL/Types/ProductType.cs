using ProductService.Application.DTOs;

namespace ProductService.API.GraphQL.Types
{
    public class ProductType : ObjectType<ProductDTO>
    {
        protected override void Configure(IObjectTypeDescriptor<ProductDTO> descriptor)
        {
            descriptor.Field(f => f.Id).Type<NonNullType<UuidType>>();
            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();
            descriptor.Field(f => f.Description).Type<StringType>();
            descriptor.Field(f => f.Price).Type<NonNullType<DecimalType>>();
            descriptor.Field(f => f.Category).Type<StringType>();
            descriptor.Field(f => f.ImageUrl).Type<StringType>();
        }
    }
}
