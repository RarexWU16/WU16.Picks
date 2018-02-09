using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu16.Picks.WEB.Models;
using Wu16.Picks.WEB.Models.Domain;

namespace Wu16.Picks.WEB.Extensions
{
    public static class ModelConversion
    {
        public static ImageViewModel ToViewModel(this Image image, string catName)
        {
            var name = image.Category == null ? catName : image.Category.Name;

            return new ImageViewModel()
            {
                Id = image.Id,
                CreationDate = image.CreationDate,
                CategoryId = image.CategoryId,
                CategoryName = name,
                FileName = image.FileName,
                Ratio = image.Ratio,
                IsInBasket = false
            };
        }
    }
}
