using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Graphic;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Authorize(Roles = "Seller,Administrator")]
[Route("cupsellclone/graphics")]
public class GraphicController : ControllerBase
{
    private readonly IGraphicService _graphicService;
    private readonly IAssetsService _imageService;

    public GraphicController(IGraphicService graphicService, IAssetsService imageService)
    {
        _graphicService = graphicService;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GraphicDto>>> Get([FromQuery] SearchQuery searchQuery)
    {
        var graphicDtos = await _graphicService.GetAll(searchQuery);
        return Ok(graphicDtos);
    }

    [HttpGet("{graphicId:guid}")]
    public async Task<ActionResult<GraphicDto>> GetById([FromRoute] Guid graphicId)
    {
        var graphicDto = await _graphicService.GetById(graphicId);
        return Ok(graphicDto);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateGraphicDto dto)
    {
        var createdGraphicId = await _graphicService.Create(dto);
        return Created(Request.Scheme + "://" + Request.Host + Url.Action("GetById", "Graphic"),
            new { graphicId = createdGraphicId });
    }

    [HttpPost("upload/{offerId:guid}")]
    public async Task<ActionResult> UploadGraphicImage([FromRoute] Guid offerId, [FromForm] IFormFile blobFile)
    {
        await _imageService.UploadOfferImage(offerId, blobFile);
        return Ok();
    }

    [HttpPut("update/{graphicId:guid}")]
    public async Task<ActionResult> Update([FromRoute] Guid graphicId, [FromQuery] string newName)
    {
        await _graphicService.Update(graphicId, newName);
        return Ok();
    }

    [HttpDelete("delete/{graphicId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid graphicId)
    {
        await _graphicService.Delete(graphicId);
        return Ok();
    }
}