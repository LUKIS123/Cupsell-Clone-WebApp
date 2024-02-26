using System.Text;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Models;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Dapper;
using Microsoft.IdentityModel.Tokens;

namespace CupsellCloneAPI.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Product?> GetById(Guid productId)
    {
        const string sql = $@"
SELECT TOP 1
    P.Id as {nameof(Product.Id)},
    P.Name as {nameof(Product.Name)},
    P.Description as {nameof(Product.Description)},
    P.ProductTypeId as {nameof(Product.ProductTypeId)},
    P.SellerId as {nameof(Product.SellerId)},
    T.Id as {nameof(ProductType.Id)},
    T.Name as {nameof(ProductType.Name)}
FROM [products].[Products] P
    INNER JOIN [products].[ProductTypes] T
    ON P.ProductTypeId = T.Id
WHERE P.Id = @Id";

        var result = await QueryProductsAsync(sql, new { Id = productId });
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Product>> GetFiltered(string? searchPhrase, int pageNumber, int pageSize,
        FilterOptionEnum sortBy, SortDirectionEnum sortDirection, Guid sellerId)
    {
        var sb = new StringBuilder($@"
SELECT
    P.Id as {nameof(Product.Id)},
    P.Name as {nameof(Product.Name)},
    P.Description as {nameof(Product.Description)},
    P.ProductTypeId as {nameof(Product.ProductTypeId)},
    T.Id as {nameof(ProductType.Id)},
    T.Name as {nameof(ProductType.Name)}
FROM [products].[Products] P
    INNER JOIN [products].[ProductTypes] T
    ON P.ProductTypeId = T.Id
");
        if (!searchPhrase.IsNullOrEmpty())
        {
            sb.Append($@"
WHERE LOWER(P.Name) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(P.Description) LIKE '%{searchPhrase?.ToLower()}%'
    OR LOWER(T.Name) LIKE '%{searchPhrase?.ToLower()}%'
");
        }

        var columnsSelectors = new Dictionary<FilterOptionEnum, string>
        {
            { FilterOptionEnum.ProductName, "ORDER BY P.Name" },
            { FilterOptionEnum.ProductDescription, "ORDER BY P.Description" },
            { FilterOptionEnum.ProductType, "ORDER BY T.Name" },
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

        return await QueryProductsAsync(
            sb.ToString(),
            new { OffsetRows = pageSize * (pageNumber - 1), FetchRows = pageSize }
        );
    }

    public async Task<Guid> Create(string name, string? description, int typeId, Guid sellerId)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        var newGuid = Guid.NewGuid();
        const string sql = @"
INSERT INTO [products].[Products]
(Id, Name, Description, ProductTypeId, SellerId)
VALUES
(@Id, @Name, @Description, @ProductTypeId, @SellerId)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Id = newGuid, Name = name, Description = description, ProductTypeId = typeId, SellerId = sellerId
            }
        );

        return newGuid;
    }

    public async Task Delete(Guid id)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [products].[Products]
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id });
    }

    public async Task Update(Guid id, string name, string? description, int typeId)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
UPDATE [products].[Products]
SET
    Name = @Name,
    Description = @Description,
    ProductTypeId = @ProductTypeId
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id, Name = name, Description = description, ProductTypeId = typeId }
        );
    }

    private async Task<IEnumerable<Product>> QueryProductsAsync(string sql, object param)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        return await conn.QueryAsync<Product, ProductType, Product>(
            sql: sql,
            map: (prod, type) =>
            {
                prod.ProductType = type;
                return prod;
            },
            param: param,
            splitOn: nameof(ProductType.Id)
        );
    }

    public async Task<IEnumerable<ProductType>> GetProductTypes()
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1000
    Id as {nameof(ProductType.Id)},
    Name as {nameof(ProductType.Name)}
FROM [products].[ProductTypes]";

        return await conn.QueryAsync<ProductType>(
            sql: sql
        );
    }
}