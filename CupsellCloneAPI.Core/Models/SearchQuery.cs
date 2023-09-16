using CupsellCloneAPI.Database.Models;

namespace CupsellCloneAPI.Core.Models
{
    public class SearchQuery
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FilterOptionEnum SortBy { get; set; } = FilterOptionEnum.None;
        public SortDirectionEnum SortDirection { get; set; } = SortDirectionEnum.ASC;
    }
}