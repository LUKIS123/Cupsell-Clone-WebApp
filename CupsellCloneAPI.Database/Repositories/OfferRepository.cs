using System.Data;
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
        const string query = $@"
SELECT TOP 1
    O.Id as {nameof(Offer.Id)},
    O.ProductId as {nameof(Offer.ProductId)},
    O.GraphicsId as {nameof(Offer.GraphicId)},
    O.SellerId as {nameof(Offer.SellerId)},
    O.Price as {nameof(Offer.Price)},
    O.IsAvailable as {nameof(Offer.IsAvailable)},
    O.Description as {nameof(Offer.Description)},
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

        using var conn = await _connectionFactory.GetSqlDbConnection();
        var result = await QueryOffersAsync(conn, query, new { Id = id });
        return result.FirstOrDefault();
    }

    public async Task<(IEnumerable<Offer>, int)> GetFiltered(string? searchPhrase, int pageNumber, int pageSize,
        FilterOptionEnum sortBy, SortDirectionEnum sortDirection)
    {
        var querySb = new StringBuilder($@"
SELECT
    O.Id as {nameof(Offer.Id)},
    O.ProductId as {nameof(Offer.ProductId)},
    O.GraphicsId as {nameof(Offer.GraphicId)},
    O.SellerId as {nameof(Offer.SellerId)},
    O.Price as {nameof(Offer.Price)},
    O.IsAvailable as {nameof(Offer.IsAvailable)},
    O.Description as {nameof(Offer.Description)},
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
    ON U.RoleId = R.Id");

        var countQuerySb = new StringBuilder($@"
SELECT
    COUNT(*) 
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
    ON U.RoleId = R.Id");

        if (!searchPhrase.IsNullOrEmpty())
        {
            var searchQuery = $@"
WHERE LOWER(P.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(P.Description) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(T.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(G.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(U.Username) LIKE '%{searchPhrase?.ToLower()}%'";

            querySb.Append(searchQuery);
            countQuerySb.Append(searchQuery);
        }

        var columnsSelectors = new Dictionary<FilterOptionEnum, string>
        {
            { FilterOptionEnum.ProductName, "ORDER BY P.Name" },
            { FilterOptionEnum.ProductDescription, "ORDER BY P.Description" },
            { FilterOptionEnum.ProductType, "ORDER BY T.Name" },
            { FilterOptionEnum.GraphicName, "ORDER BY G.Name" },
            { FilterOptionEnum.SellerUsername, "ORDER BY U.Username" },
        };

        querySb.Append(sortBy != FilterOptionEnum.None
            ? columnsSelectors[sortBy]
            : columnsSelectors[FilterOptionEnum.ProductName]);

        if (sortDirection == SortDirectionEnum.DESC)
        {
            querySb.AppendJoin(" ", "DESC");
        }

        querySb.Append(@"
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY");

        using var conn = await _connectionFactory.GetSqlDbConnection();
        return await QueryOffersWithCountAsync(conn, querySb.ToString(),
            new { OffsetRows = pageSize * (pageNumber - 1), FetchRows = pageSize }, countQuerySb.ToString()
        );
    }

    public async Task<Guid> Create(Guid productId, Guid graphicId, Guid sellerId, decimal price, bool isAvailable,
        string? description)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        var newGuid = Guid.NewGuid();
        const string sql = @"
INSERT INTO [products].[Offers]
(Id, ProductId, GraphicsId, SellerId, Price, IsAvailable, Description)
VALUES
(@Id, @ProductId, @GraphicsId, @SellerId, @Price, @IsAvailable, @Description)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Id = newGuid, ProductId = productId, GraphicId = graphicId, SellerId = sellerId, Price = price,
                IsAvailable = isAvailable, Description = description
            }
        );
        return newGuid;
    }

    public async Task Delete(Guid id)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [products].[Offers]
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id });
    }

    public async Task Update(Guid id, Guid productId, Guid graphicId, Guid sellerId, decimal price, bool isAvailable,
        string? description)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
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

    private Task<IEnumerable<Offer>> QueryOffersAsync(IDbConnection dbConnection, string query, object param)
    {
        return dbConnection.QueryAsync<Offer, Product, ProductType, Graphic, User, Role, Offer>(
            sql: query,
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

    private async Task<(IEnumerable<Offer> Offers, int Count)> QueryOffersWithCountAsync(IDbConnection dbConnection,
        string query, object param, string countQuery)
    {
        var resultsTask = QueryOffersAsync(dbConnection, query, param);
        var countTask = dbConnection.ExecuteScalarAsync<int>(countQuery);

        await Task.WhenAll(resultsTask, countTask);
        return (resultsTask.Result, countTask.Result);
    }
}