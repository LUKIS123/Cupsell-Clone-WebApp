using AutoMapper;
using CupsellCloneAPI.Core.Models.Dtos.Graphic;
using CupsellCloneAPI.Core.Models.Dtos.Offer;
using CupsellCloneAPI.Core.Models.Dtos.Product;
using CupsellCloneAPI.Core.Models.Dtos.User;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Entities.User;

namespace CupsellCloneAPI.Core.MappingProfiles
{
    public class ModelMappingProfile : Profile
    {
        public ModelMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.ProductTypeName,
                    m => m.MapFrom(x => x.ProductType.Name)
                );

            CreateMap<Graphic, GraphicDto>()
                .ForMember(d => d.SellerName,
                    m => m.MapFrom(x => x.Seller.Username)
                );

            CreateMap<User, UserDto>()
                .ForMember(d => d.RoleName,
                    m => m.MapFrom(x => x.Role.Name)
                );

            CreateMap<Offer, OfferDto>()
                .ForMember(d => d.Product,
                    m => m.MapFrom(x => new ProductDto()
                    {
                        Id = x.ProductId,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        ProductTypeName = x.Product.ProductType.Name
                    }))
                .ForMember(d => d.Graphic,
                    m => m.MapFrom(x => new GraphicDto()
                    {
                        Id = x.GraphicId,
                        Name = x.Graphic.Name,
                        SellerName = x.Graphic.Seller.Username
                    }))
                .ForMember(d => d.Seller,
                    m => m.MapFrom(x => new UserDto()
                    {
                        Id = x.SellerId,
                        Email = x.Seller.Email,
                        Username = x.Seller.Username,
                        PhoneNumber = x.Seller.PhoneNumber,
                        Address = x.Seller.Address,
                        RoleName = x.Seller.Role.Name
                    }));
        }
    }
}