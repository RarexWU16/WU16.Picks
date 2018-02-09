using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wu16.Picks.WEB.Models.Domain
{ 
    public class Image
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime CreationDate { get; set; }
        public double Ratio { get; set; }

        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}