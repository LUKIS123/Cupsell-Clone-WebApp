using System.Data;
using System.Text;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Models;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Dapper;

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
        const string query = $@"
SELECT TOP 1
    P.Id as {nameof(Product.Id)},
    P.Name as {nameof(Product.Name)},
    P.Description as {nameof(Product.Description)},
    P.ProductTypeId as {nameof(Product.ProductTypeId)},
    T.Id as {nameof(ProductType.Id)},
    T.Name as {nameof(ProductType.Name)}
FROM [products].[Products] P
    INNER JOIN [products].[ProductTypes] T
    ON P.ProductTypeId = T.Id
WHERE P.Id = @Id";

        using var conn = await _connectionFactory.GetSqlDbConnection();
        var result = await QueryProductsAsync(conn, query, new { Id = productId });
        return result.FirstOrDefault();
    }

    public async Task<(IEnumerable<Product> Products, int Count)> GetFiltered(string? searchPhrase, int pageNumber,
        int pageSize, FilterOptionEnum sortBy, SortDirectionEnum sortDirection)
    {
        var querySb = new StringBuilder($@"
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
        var countQuerySb = new StringBuilder($@"
SELECT
    COUNT(*)
FROM [products].[Products] P
    INNER JOIN [products].[ProductTypes] T
    ON P.ProductTypeId = T.Id
");

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            var searchQuery = $@"
WHERE LOWER(P.Name) LIKE '%{searchPhrase.ToLower()}%'
    OR LOWER(P.Description) LIKE '%{searchPhrase.ToLower()}%'
    OR LOWER(T.Name) LIKE '%{searchPhrase.ToLower()}%'
";

            querySb.Append(searchQuery);
            countQuerySb.Append(searchQuery);
        }

        var columnsSelectors = new Dictionary<FilterOptionEnum, string>
        {
            { FilterOptionEnum.ProductName, "ORDER BY P.Name" },
            { FilterOptionEnum.ProductDescription, "ORDER BY P.Description" },
            { FilterOptionEnum.ProductType, "ORDER BY T.Name" },
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
        return await QueryProductsWithCountAsync(conn, querySb.ToString(),
            new { OffsetRows = pageSize * (pageNumber - 1), FetchRows = pageSize }, countQuerySb.ToString()
        );
    }

    public async Task<Guid> Create(string name, string? description, int typeId)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        var newGuid = Guid.NewGuid();
        const string sql = @"
INSERT INTO [products].[Products]
(Id, Name, Description, ProductTypeId)
VALUES
(@Id, @Name, @Description, @ProductTypeId)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = newGuid, Name = name, Description = description, ProductTypeId = typeId }
        );

        return newGuid;
    }

    public async Task Delete(Guid id)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [products].[Products]
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id });
    }

    public async Task Update(Guid id, string name, string? description, int typeId)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
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

    private Task<IEnumerable<Product>> QueryProductsAsync(IDbConnection dbConnection, string query, object param)
    {
        return dbConnection.QueryAsync<Product, ProductType, Product>(
            sql: query,
            map: (prod, type) =>
            {
                prod.ProductType = type;
                return prod;
            },
            param: param,
            splitOn: nameof(ProductType.Id)
        );
    }

    private async Task<(IEnumerable<Product> Products, int Count)> QueryProductsWithCountAsync(
        IDbConnection dbConnection, string query, object param, string countQuery)
    {
        var resultsTask = QueryProductsAsync(dbConnection, query, param);
        var countTask = dbConnection.ExecuteScalarAsync<int>(countQuery);

        await Task.WhenAll(resultsTask, countTask);
        return (resultsTask.Result, countTask.Result);
    }

    public async Task<IEnumerable<ProductType>> GetProductTypes()
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
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