using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Wu16.Picks.WEB.Extensions;
using System.IO.Compression;
using System.IO;
using Wu16.Picks.WEB.Models;
using Wu16.Picks.WEB.Repositories;
using Microsoft.Extensions.Configuration;

namespace Wu16.Picks.WEB.Controllers
{
    public class ApiController : Controller
    {
        private ImageRepository _images;
        private CategoryRepository _categories;
        private IDistributedCache _cache;
        private string _basePath;

        public ApiController(ImageRepository images, CategoryRepository categories, IDistributedCache cache, IConfiguration conf)
        {
            _images = images;
            _categories = categories;
            _cache = cache;
            _basePath = conf.GetValue<string>("base-path");
        }

        [HttpGet, Route("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _categories.Get());
        }

        [HttpPost, Route("api/basket/toggle/{id}")]
        public bool ToggleBasket(Guid id)
        {
            var basket = HttpContext.Session.Get<HashSet<Guid>>("basket");

            if (basket == null)
                basket = new HashSet<Guid>();

            var contains = basket.Contains(id);

            basket = _images.ToggleBasket(id, basket);

            HttpContext.Session.Set("basket", basket);

            return !contains;
        }


        [HttpGet, Route("api/basket")]
        public async Task<IEnumerable<ImageViewModel>> GetBasket()
        {
            var basket = HttpContext.Session.Get<HashSet<Guid>>("basket");

            if (basket == null)
                return new List<ImageViewModel>();

            return await _images.GetImages(basket);
        }

        [HttpGet, Route("api/images/{page}/{categoryId?}")]
        public async Task<IEnumerable<ImageViewModel>> GetImages(int page, Guid? categoryId)
        {
            var basket = HttpContext.Session.Get<HashSet<Guid>>("basket");

            if (basket == null)
                basket = new HashSet<Guid>();

            if (categoryId == null)
                return await _images.GetLatest(page * 9, 9, basket);
            else
                return await _images.GetByCategory((Guid)categoryId, page * 9, 9, basket);
        }

        [HttpGet, Route("api/zipfile")]
        public async Task<IActionResult> GetZipFile()
        {
            var basket = HttpContext.Session.Get<HashSet<Guid>>("basket");

            if (basket == null || basket.Count == 0)
                return BadRequest();

            var images = await _images.GetImages(basket);

            byte[] bytes;

            using (var ms = new MemoryStream())
            {
                using (var imagezip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    foreach (var image in images)
                        imagezip.CreateEntryFromFile($"{_basePath}{image.Id}/{image.FileName}", image.FileName, CompressionLevel.Fastest);

                ms.Position = 0;
                bytes = ms.ToArray();
            }

            return File(bytes, "application/zip", "images.zip");
        }

        [HttpPost, Route("api/upload-image")]
        public async Task<IActionResult> UploadImage(ImageUploadViewModel model)
        {
            try
            {
                if (model.CategoryId == null && string.IsNullOrEmpty(model.CategoryName))
                    return BadRequest();

                if (model.CategoryId == null)
                    model.CategoryId = Guid.NewGuid();

                if (!model.Image.ContentType.Contains("image"))
                    return BadRequest("file is not a image!");

                await _images.UploadImage(model);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.ToString());
            }
        }
    }
}
