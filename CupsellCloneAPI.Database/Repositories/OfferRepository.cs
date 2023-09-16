using System.Text;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Entities.User;
using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Models;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Dapper;
using Microsoft.IdentityModel.Tokens;

namespace CupsellCloneAPI.Database.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OfferRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Offer?> GetById(Guid id)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1
    O.Id as {nameof(Offer.Id)},
    O.ProductId as {nameof(Offer.ProductId)},
    O.GraphicsId as {nameof(Offer.GraphicId)},
    O.SellerId as {nameof(Offer.SellerId)},
    O.Price as {nameof(Offer.Price)},
    O.IsAvailable as {nameof(Offer.IsAvailable)},
    P.Id as {nameof(Product.Id)},
    P.Name as {nameof(Product.Name)},
    P.Description as {nameof(Product.Description)},
    P.ProductTypeId as {nameof(Product.ProductTypeId)},
    T.Id as {nameof(ProductType.Id)},
    T.Name as {nameof(ProductType.Name)},
    G.Id as {nameof(Graphic.Id)},
    G.Name as {nameof(Graphic.Name)},
    G.SellerId as {nameof(Graphic.SellerId)},
    U.Id as {nameof(User.Id)},
    U.Email as {nameof(User.Email)},
    U.Username as {nameof(User.Username)},
    U.PasswordHash as {nameof(User.PasswordHash)},
    U.PhoneNumber as {nameof(User.PhoneNumber)},
    U.Name as {nameof(User.Name)},
    U.LastName as {nameof(User.LastName)},
    U.DateOfBirth as {nameof(User.DateOfBirth)},
    U.Address as {nameof(User.Address)},
    U.RoleId as {nameof(User.RoleId)},
    R.Id as {nameof(Role.Id)},
    R.Name as {nameof(Role.Name)}
FROM [products].[Offers] O
    INNER JOIN [products].[Products] P
    ON O.ProductId = P.Id
    INNER JOIN [products].[ProductTypes] T
    ON P.ProductTypeId = T.Id
    INNER JOIN [products].[Graphics] G
    ON O.GraphicsId = G.Id
    INNER JOIN [users].[Users] U
    ON O.SellerId = U.Id
    INNER JOIN [users].[Roles] R
    ON U.RoleId = R.Id
WHERE O.Id = @Id";

        var result = await QueryOffersAsync(sql, new { Id = id });
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Offer>> GetFiltered(
        string? searchPhrase,
        int pageNumber,
        int pageSize,
        FilterOptionEnum sortBy,
        SortDirectionEnum sortDirection)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        var sb = new StringBuilder($@"
SELECT
    O.Id as {nameof(Offer.Id)},
    O.ProductId as {nameof(Offer.ProductId)},
    O.GraphicsId as {nameof(Offer.GraphicId)},
    O.SellerId as {nameof(Offer.SellerId)},
    O.Price as {nameof(Offer.Price)},
    O.IsAvailable as {nameof(Offer.IsAvailable)},
    P.Id as {nameof(Product.Id)},
    P.Name as {nameof(Product.Name)},
    P.Description as {nameof(Product.Description)},
    P.ProductTypeId as {nameof(Product.ProductTypeId)},
    T.Id as {nameof(ProductType.Id)},
    T.Name as {nameof(ProductType.Name)},
    G.Id as {nameof(Graphic.Id)},
    G.Name as {nameof(Graphic.Name)},
    G.SellerId as {nameof(Graphic.SellerId)},
    U.Id as {nameof(User.Id)},
    U.Email as {nameof(User.Email)},
    U.Username as {nameof(User.Username)},
    U.PasswordHash as {nameof(User.PasswordHash)},
    U.PhoneNumber as {nameof(User.PhoneNumber)},
    U.Name as {nameof(User.Name)},
    U.LastName as {nameof(User.LastName)},
    U.DateOfBirth as {nameof(User.DateOfBirth)},
    U.Address as {nameof(User.Address)},
    U.RoleId as {nameof(User.RoleId)},
    R.Id as {nameof(Role.Id)},
    R.Name as {nameof(Role.Name)}
FROM [products].[Offers] O
    INNER JOIN [products].[Products] P
    ON O.ProductId = P.Id
    INNER JOIN [products].[ProductTypes] T
    ON P.ProductTypeId = T.Id
    INNER JOIN [products].[Graphics] G
    ON O.GraphicsId = G.Id
    INNER JOIN [users].[Users] U
    ON O.SellerId = U.Id
    INNER JOIN [users].[Roles] R
    ON U.RoleId = R.Id
");
        if (!searchPhrase.IsNullOrEmpty())
        {
            sb.Append($@"
WHERE LOWER(P.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(P.Description) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(T.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(G.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(U.Username) LIKE '%{searchPhrase?.ToLower()}%'
");
        }

        var columnsSelectors = new Dictionary<FilterOptionEnum, string>
        {
            { FilterOptionEnum.ProductName, "ORDER BY P.Name" },
            { FilterOptionEnum.ProductDescription, "ORDER BY P.Description" },
            { FilterOptionEnum.ProductType, "ORDER BY T.Name" },
            { FilterOptionEnum.GraphicName, "ORDER BY G.Name" },
            { FilterOptionEnum.SellerUsername, "ORDER BY U.Username" },
        };

        sb.Append(sortBy != FilterOptionEnum.None
            ? columnsSelectors[sortBy]
            : columnsSelectors[FilterOptionEnum.ProductName]);

        if (sortDirection == SortDirectionEnum.DESC)
        {
            sb.AppendJoin(" ", "DESC");
        }

        sb.Append(@"
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY");

        return await QueryOffersAsync(
            sb.ToString(),
            new { OffsetRows = pageSize * (pageNumber - 1), FetchRows = pageSize }
        );
    }

    public async Task<Guid> Create(Guid productId, Guid graphicId, Guid sellerId, decimal price, bool isAvailable)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        var newGuid = Guid.NewGuid();
        const string sql = @"
INSERT INTO [products].[Offers]
(Id, ProductId, GraphicsId, SellerId, Price, IsAvailable)
VALUES
(@Id, @ProductId, @GraphicsId, @SellerId, @Price, @IsAvailable)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Id = newGuid, ProductId = productId, GraphicId = graphicId, SellerId = sellerId, Price = price,
                IsAvailable = isAvailable
            }
        );
        return newGuid;
    }

    public async Task Delete(Guid id)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [products].[Offers]
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id });
    }

    public async Task Update(Guid id, Guid productId, Guid graphicId, Guid sellerId, decimal price, bool isAvailable)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
UPDATE [products].[Offers]
SET
    ProductId = @ProductId,
    GraphicsId = @GraphicsId,
    SellerId = @SellerId,
    Price = @Price,
    IsAvailable = @IsAvailable
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Id = id, ProductId = productId, GraphicsId = graphicId, SellerId = sellerId, Price = price,
                IsAvailable = isAvailable
            }
        );
    }

    private async Task<IEnumerable<Offer>> QueryOffersAsync(string sql, object param)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        return await conn.QueryAsync<Offer, Product, ProductType, Graphic, User, Role, Offer>(
            sql: sql,
            map: (offer, product, type, graphic, user, role) =>
            {
                product.ProductType = type;
                offer.Product = product;
                graphic.Seller = user;
                offer.Graphic = graphic;
                user.Role = role;
                offer.Seller = user;
                return offer;
            },
            param: param,
            splitOn: nameof(Product.Id)
        );
    }
}