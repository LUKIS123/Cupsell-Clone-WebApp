using CupsellCloneAPI.Database.BlobContainer.Models;
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
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

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
        var t = _blobRepository.ListBlobs();
        await t;

        return await _offerRepository.GetById(Guid.Parse("80791029-d415-4739-afeb-75fb6fd1f5af"));
    }

    [HttpPost("upload")]
    public async Task<ActionResult> UploadFile([FromForm] IFormFile blobFile)
    {
        await _blobRepository.UploadBlobFile(blobFile, "offers/exampleOfferId");
        return Ok();
    }

    [HttpGet("download")]
    public async Task<ActionResult> DownloadFile()
    {
        var result = await _blobRepository.DownloadBlobFilesInDirectory("offers/exampleOfferId");

        var o = result.FirstOrDefault();
        var s = o.FileStream;
        var t = o.ContentType;
        return File(s, t);
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}