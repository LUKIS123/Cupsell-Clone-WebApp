using CupsellCloneAPI.Database.BlobContainer.Repositories;
using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Models;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IOfferRepository _offerRepository;
    private readonly IBlobRepository _blobRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IProductRepository productRepository,
        IOfferRepository offerRepository, IBlobRepository blobRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _offerRepository = offerRepository;
        _blobRepository = blobRepository;
    }

    [HttpGet("TEST")]
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _productRepository.GetFiltered("t-Shirt", 1, 2, FilterOptionEnum.None, SortDirectionEnum.ASC);
    }

    [HttpGet("TEST2")]
    public async Task<Offer?> GetOffer()
    {
        return await _offerRepository.GetById(Guid.Parse("80791029-d415-4739-afeb-75fb6fd1f5af"));
    }

    [HttpPost("upload")]
    public async Task<ActionResult> UploadFile([FromForm] IFormFile blobFile)
    {
        await _blobRepository.UploadBlobFile(blobFile, "offers/exampleOfferId");
        return Ok();
    }

    [HttpGet("getURLs")]
    public async Task<ActionResult<IEnumerable<string>>> GetUrls()
    {
        var result = await _blobRepository.ListBlobs("offers/exampleOfferId");
        return Ok(result);
    }

    [HttpGet("download")]
    public async Task<ActionResult> DownloadFile([FromQuery] string path)
    {
        var result = await _blobRepository.DownloadBlobFile(path);
        return File(result.FileStream, result.ContentType);
    }
}