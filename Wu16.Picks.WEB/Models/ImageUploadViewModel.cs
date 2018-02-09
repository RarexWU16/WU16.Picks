using System;
using Microsoft.AspNetCore.Http;

namespace Wu16.Picks.WEB.Models
{
    public class ImageUploadViewModel
    {
        public IFormFile Image { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
