using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wu16.Picks.WEB.DataAccess;
using Wu16.Picks.WEB.Extensions;
using Wu16.Picks.WEB.Models;

namespace Wu16.Picks.WEB.Repositories
{
    public class ImageRepository
    {
        private const string _key = "images";

        private string _basePath;
        private CategoryRepository _categories;
        private ApplicationDbContext _dbContext;
        private IDistributedCache _cache;

        public ImageRepository(CategoryRepository categories, ApplicationDbContext dbContext, IDistributedCache cache, IConfiguration configuration)
        {
            _categories = categories;
            _dbContext = dbContext;
            _cache = cache;

            _basePath = configuration.GetValue<string>("base-path");
        }

        private List<ImageViewModel> GetCache()
            => _cache.Get<List<ImageViewModel>>(_key);
        private void SetCache(List<ImageViewModel> toCache)
            => _cache.Set(_key, toCache);

        private void SetCategoryCache(Guid categoryId, List<ImageViewModel> toCache)
            => _cache.Set($"{_key}:category:{categoryId}", toCache);
        private List<ImageViewModel> GetCategoryCache(Guid categoryId)
            => _cache.Get<List<ImageViewModel>>($"{_key}:category:{categoryId}");

        private static System.Drawing.Image GetThumbnail(System.Drawing.Image img)
        {
            var ratio = 32 / (double)img.Width;

            return new Bitmap(img, (int)(img.Width * ratio), (int)(img.Height * ratio));
        }

        public HashSet<Guid> ToggleBasket(Guid id, HashSet<Guid> basket)
        {
            if (!basket.Contains(id))
            {
                basket.Add(id);
                return basket;
            }

            basket.Remove(id);
            return basket;
        }

        public async Task UploadImage(ImageUploadViewModel model)
        {
            var categoryId = model.CategoryId ?? Guid.NewGuid();

            if (!_categories.IsCategory(categoryId))
                await _categories.Add(categoryId, model.CategoryName);

            var dbImage = new Models.Domain.Image()
            {
                Id = Guid.NewGuid(),
                CategoryId = categoryId,
                ContentType = model.Image.ContentType,
                FileName = model.Image.FileName,
                CreationDate = DateTime.Now
            };

            Directory.CreateDirectory($"{_basePath}{dbImage.Id}");

            using (var ms = new MemoryStream())
            using (var rs = model.Image.OpenReadStream())
            {
                await rs.CopyToAsync(ms);

                ms.Position = 0;
                using (var bImage = System.Drawing.Image.FromStream(ms))
                {
                    dbImage.Ratio = (double)bImage.Height / (double)bImage.Width;

                    var thumbnail = GetThumbnail(bImage);

                    ms.Position = 0;
                    bImage.Save($"{_basePath}{dbImage.Id}/{dbImage.FileName}");
                    thumbnail.Save($"{_basePath}{dbImage.Id}/sm-{dbImage.FileName}");
                }
            }

            var imagesCached = GetCache();
            var categoryimagesCached = GetCategoryCache(categoryId);

            var vm = dbImage.ToViewModel(model.CategoryName);

            // update image cache
            if (imagesCached != null)
            {
                imagesCached.Add(vm);

                SetCache(imagesCached);
            }

            // update category image cache
            if (categoryimagesCached != null)
            {
                categoryimagesCached.Add(vm);

                SetCategoryCache(categoryId, categoryimagesCached);
            }

            await _dbContext.Images.AddAsync(dbImage);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<List<ImageViewModel>> Get(Guid? categoryId = null, HashSet<Guid> basket = null)
        {
            List<ImageViewModel> result;

            var shouldSet = false;

            if (categoryId == null) result = GetCache();
            else result = GetCategoryCache((Guid)categoryId);

            if (result == null)
            {
                var query = _dbContext.Images
                   .Include(x => x.Category)
                   .AsQueryable();

                if (categoryId != null)
                    query = query.Where(x => x.CategoryId == categoryId);

                result = await query
                     .Select(x => x.ToViewModel(null))
                     .ToListAsync();

                shouldSet = true;
            }

            if (shouldSet)
            {
                if (categoryId == null)
                    SetCache(result);
                else
                    SetCategoryCache((Guid)categoryId, result);
            }

            if (basket != null)
                result.ForEach(x => x.IsInBasket = basket.Contains(x.Id));

            return result
                .OrderByDescending(x => x.CreationDate)
                .ToList();
        }

        public async Task<IEnumerable<ImageViewModel>> GetImages(HashSet<Guid> basket)
            => (await Get(null, basket))
                .Where(x => x.IsInBasket).ToList();

        public async Task<List<ImageViewModel>> GetLatest(int skip, int take, HashSet<Guid> basket)
            => (await Get(null, basket))
                .Skip(skip)
                .Take(take)
                .ToList();

        public async Task<List<ImageViewModel>> GetByCategory(Guid categoryId, int skip, int take, HashSet<Guid> basket)
            => (await Get(categoryId, basket))
                .Skip(skip)
                .Take(take)
                .ToList();
    }
}
