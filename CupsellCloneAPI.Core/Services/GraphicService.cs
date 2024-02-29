using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Graphic;
using CupsellCloneAPI.Core.Services.Interfaces;

namespace CupsellCloneAPI.Core.Services
{
    internal class GraphicService : IGraphicService
    {
        public Task<PageResult<GraphicDto>> GetAll(SearchQuery searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task<GraphicDto> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Create(CreateGraphicDto newGraphic)
        {
            throw new NotImplementedException();
        }

        public Task Update(Guid id, string graphicName)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}