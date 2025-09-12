using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
namespace ProductAPI.Model
{
    public class Product
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public required decimal Price { get; set; }
    }
}

