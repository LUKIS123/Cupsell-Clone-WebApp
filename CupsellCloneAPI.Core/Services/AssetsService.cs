﻿using CupsellCloneAPI.Core.Exceptions;
using CupsellCloneAPI.Core.Services.Interfaces;
using CupsellCloneAPI.Database.BlobContainer.Models;
using CupsellCloneAPI.Database.BlobContainer.Repositories;
using Microsoft.AspNetCore.Http;

namespace CupsellCloneAPI.Core.Services;

internal class AssetsService : IAssetsService
{
    private readonly IBlobRepository _blobRepository;
    public static readonly List<string> ImageExtensions = [".JPG", ".JPEG", ".PNG"];

    public static readonly string OfferCatalog = "offers";
    public static readonly string GraphicCatalog = "graphics";
    public static readonly string ProductCatalog = "products";

    public AssetsService(IBlobRepository blobRepository)
    {
        _blobRepository = blobRepository;
    }

    public async Task<Dictionary<Guid, IEnumerable<string>>> GetOffersImagesUris(IEnumerable<Guid> offersId)
    {
        return await _blobRepository
            .ListBlobsByGuids(OfferCatalog, offersId);
    }

    public async Task<Dictionary<Guid, string>> GetGraphicsImagesUris(IEnumerable<Guid> graphicId)
    {
        return await _blobRepository
            .ListBlobByGuids(GraphicCatalog, graphicId);
    }

    public async Task<Dictionary<Guid, string>> GetProductsImagesUris(IEnumerable<Guid> productId)
    {
        return await _blobRepository
            .ListBlobByGuids(ProductCatalog, productId);
    }

    public async Task<IEnumerable<string>> GetOfferImageUris(Guid offerId)
    {
        return await _blobRepository
            .ListBlobs(OfferCatalog, offerId);
    }

    public async Task<string?> GetGraphicImageUri(Guid graphicId)
    {
        return await _blobRepository
            .ListBlob(GraphicCatalog, graphicId);
    }

    public async Task<string?> GetProductImageUri(Guid productId)
    {
        return await _blobRepository
            .ListBlob(ProductCatalog, productId);
    }

    public async Task<BlobObject> GetOfferImage(Guid offerId, string imageName)
    {
        var path = $"{OfferCatalog}/{offerId}/{imageName}";
        var streamContentTypeTuple = await _blobRepository.DownloadBlobFile(path);
        return MapToBlobObject(streamContentTypeTuple.Item1, streamContentTypeTuple.Item2, path);
    }

    public async Task<BlobObject> GetGraphicImage(Guid graphicId, string imageName)
    {
        var path = $"{GraphicCatalog}/{graphicId}/{imageName}";
        var streamContentTypeTuple = await _blobRepository.DownloadBlobFile(path);
        return MapToBlobObject(streamContentTypeTuple.Item1, streamContentTypeTuple.Item2, path);
    }

    public async Task<BlobObject> GetProductImage(Guid productId, string imageName)
    {
        var path = $"productTypes/{productId}/{imageName}";
        var streamContentTypeTuple = await _blobRepository.DownloadBlobFile(path);
        return MapToBlobObject(streamContentTypeTuple.Item1, streamContentTypeTuple.Item2, path);
    }

    public async Task<string> UploadOfferImage(Guid offerId, IFormFile blobFile)
    {
        var bytes = await GetBytesFromIFormFile(blobFile);

        var basePath = $"{OfferCatalog}/{offerId}";
        return await _blobRepository.UploadBlobFile(bytes, blobFile.FileName, basePath);
    }

    public async Task<string> UploadProductImage(Guid productId, IFormFile blobFile)
    {
        var bytes = await GetBytesFromIFormFile(blobFile);

        var basePath = $"{ProductCatalog}/{productId}";
        return await _blobRepository.UploadBlobFile(bytes, blobFile.FileName, basePath);
    }

    private static BlobObject MapToBlobObject(Stream stream, string contentDetailsType, string uri)
    {
        var fileName = uri.Split("/").Last();

        string contentType;
        if (ImageExtensions.Contains(Path.GetExtension(fileName.ToUpperInvariant())))
        {
            var extension = Path.GetExtension(fileName);
            contentType = "image/" + extension.Remove(0, 1);
        }
        else
        {
            contentType = contentDetailsType;
        }

        return new BlobObject()
        {
            FileStream = stream,
            ContentType = contentType
        };
    }

    private static async Task<byte[]> GetBytesFromIFormFile(IFormFile formFile)
    {
        var length = formFile.Length;
        if (length < 0)
        {
            throw new BadFileException("File length is less than 0");
        }

        await using var fileStream = formFile.OpenReadStream();
        var bytes = new byte[length];
        var read = await fileStream.ReadAsync(bytes.AsMemory(0, (int)formFile.Length));
        if (read != length)
        {
            throw new BadFileException("File length is not equal to read bytes");
        }

        return bytes;
    }
}