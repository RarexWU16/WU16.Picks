using System;
using System.Collections.Generic;

namespace Wu16.Picks.WEB.Models.Domain
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Image> Images { get; set; }
    }
}