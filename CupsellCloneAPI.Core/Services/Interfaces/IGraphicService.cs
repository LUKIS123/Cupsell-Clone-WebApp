using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Graphic;

namespace CupsellCloneAPI.Core.Services.Interfaces
{
    public interface IGraphicService
    {
        Task<PageResult<GraphicDto>> GetAll(SearchQuery searchQuery);
        Task<GraphicDto> GetById(Guid id);
        Task<Guid> Create(CreateGraphicDto newGraphic);
        Task Update(Guid id, string graphicName);
        Task Delete(Guid id);
    }
}