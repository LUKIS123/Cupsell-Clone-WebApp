namespace CupsellCloneAPI.Core.Models
{
    public class PageResult<T>
    {
        public required IEnumerable<T> Items { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItemsCount / (double)PageSize);
        public int ItemsFrom => PageSize * (PageNumber - 1) + 1;
        public int ItemsTo => ItemsFrom + PageSize - 1;
        public int TotalItemsCount { get; init; }
        public int PageSize { get; init; }
        public int PageNumber { get; init; }
    }
}