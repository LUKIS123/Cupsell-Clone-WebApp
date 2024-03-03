using CupsellCloneAPI.Core.Models;
using CupsellCloneAPI.Core.Models.Dtos.Product;
using CupsellCloneAPI.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/cupsellclone/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IAssetsService _imageService;

    public ProductController(IProductService productService, IAssetsService imageService)
    {
        _productService = productService;
        _imageService = imageService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SearchQuery searchQuery)
    {
        var products = await _productService.GetProducts(searchQuery);
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid productId)
    {
        var product = await _productService.GetProductById(productId);
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var createdProductId = await _productService.CreateProduct(dto);
        return Created(Request.Scheme + "://" + Request.Host + Url.Action("GetById", "Product"),
            new { productId = createdProductId });
    }

    [HttpPost("upload/{productId:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UploadProductImage([FromRoute] Guid productId, [FromForm] IFormFile blobFile)
    {
        await _imageService.UploadProductImage(productId, blobFile);
        return Ok();
    }

    [HttpPut("update/{productId:guid}")]
    [Authorize("Administrator")]
    public async Task<IActionResult> Update([FromRoute] Guid productId, [FromBody] UpdateProductDto dto)
    {
        await _productService.UpdateProduct(productId, dto);
        return Ok();
    }

    [HttpDelete("{productId:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete([FromRoute] Guid productId)
    {
        await _productService.DeleteProduct(productId);
        return Ok();
    }
}