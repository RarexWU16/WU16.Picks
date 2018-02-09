using System;

namespace Wu16.Picks.WEB.Models
{
    public class ImageViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime CreationDate { get; set; }
        public double Ratio { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsInBasket { get; set; }
    }
}