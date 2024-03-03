using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Graphic;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IGraphicService
    {
        Task<PageResult<GraphicDto>> GetByUser(SearchQuery searchQuery);
        Task<GraphicDto> GetById(Guid id);
        Task<Guid> Create(CreateGraphicDto newGraphic);
        Task Update(Guid id, UpdateGraphicDto updatedGraphic);
        Task Delete(Guid id);
    }
}