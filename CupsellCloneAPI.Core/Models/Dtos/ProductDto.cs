﻿namespace CupsellCloneAPI.Core.Models.Dtos
{
    public class ProductDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string ProductTypeName { get; set; }
    }
}