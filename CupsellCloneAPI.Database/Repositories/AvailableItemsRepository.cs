using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Dapper;

namespace CupsellCloneAPI.Database.Repositories;

public class AvailableItemsRepository : IAvailableItemsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AvailableItemsRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AvailableItem?> GetById(Guid id)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1
    A.Id as {nameof(AvailableItem.Id)},
    A.OfferId as {nameof(AvailableItem.OfferId)},
    A.SizeId as {nameof(AvailableItem.SizeId)},
    A.Quantity as {nameof(AvailableItem.Quantity)},
    S.Id as {nameof(Size.Id)},
    S.Name as {nameof(Size.Name)}
FROM [products].[AvailableItems] A
    INNER JOIN [products].[Sizes] S
    ON A.SizeId = S.Id
WHERE A.Id = @Id";

        var result = await conn.QueryAsync<AvailableItem, Size, AvailableItem>(
            sql: sql,
            map: (item, size) =>
            {
                item.Size = size;
                return item;
            },
            param: new { Id = id },
            splitOn: nameof(Size.Id)
        );
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<AvailableItem>> GetAvailableItemsByOfferId(Guid offerId)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT
    A.Id as {nameof(AvailableItem.Id)},
    A.OfferId as {nameof(AvailableItem.OfferId)},
    A.SizeId as {nameof(AvailableItem.SizeId)},
    A.Quantity as {nameof(AvailableItem.Quantity)},
    S.Id as {nameof(Size.Id)},
    S.Name as {nameof(Size.Name)}
FROM [products].[AvailableItems] A
    INNER JOIN [products].[Sizes] S
    ON A.SizeId = S.Id
WHERE A.OfferId = @OfferId";

        return await conn.QueryAsync<AvailableItem, Size, AvailableItem>(
            sql: sql,
            map: (item, size) =>
            {
                item.Size = size;
                return item;
            },
            param: new { OfferId = offerId },
            splitOn: nameof(Size.Id)
        );
    }

    public async Task<Dictionary<Size, Guid>> CreateItems(Dictionary<Size, int> sizeQuantityDictionary, Guid offerId)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        var sizeGuidDictionary = new Dictionary<Size, Guid>();
        const string sql = @"
INSERT INTO [products].[AvailableItems]
(Id, OfferId, SizeId, Quantity)
VALUES
(@Id, @OfferId, @SizeId, @Quantity)";

        foreach (var keyValuePair in sizeQuantityDictionary)
        {
            var newGuid = Guid.NewGuid();
            await conn.ExecuteAsync(
                sql: sql,
                param: new
                {
                    Id = newGuid, OfferId = offerId, SizeId = keyValuePair.Key.Id, Quantity = keyValuePair.Value
                }
            );
            sizeGuidDictionary[keyValuePair.Key] = newGuid;
        }

        return sizeGuidDictionary;
    }

    public async Task Delete(Guid id)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [products].[AvailableItems]
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id });
    }

    public async Task Update(Guid id, int sizeId, int quantity)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
UPDATE [products].[AvailableItems]
SET
    SizeId = @SizeId,
    Quantity = @Quantity
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Id = id,
                SizeId = sizeId,
                Quantity = quantity
            }
        );
    }

    public async Task<IEnumerable<Size>> GetSizes()
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1000
    Id as {nameof(Size.Id)},
    Name as {nameof(Size.Name)}
FROM [products].[Sizes]";

        return await conn.QueryAsync<Size>(
            sql: sql
        );
    }
}