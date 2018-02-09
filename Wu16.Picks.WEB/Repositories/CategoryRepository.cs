using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu16.Picks.WEB.DataAccess;
using Wu16.Picks.WEB.Extensions;
using Wu16.Picks.WEB.Models;
using Wu16.Picks.WEB.Models.Domain;

namespace Wu16.Picks.WEB.Repositories
{
    public class CategoryRepository
    {
        private const string _key = "categories";
        private ApplicationDbContext _dbContext;
        private IDistributedCache _cache;

        public CategoryRepository(ApplicationDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        private List<CategoryViewModel> GetCache()
            => _cache.Get<List<CategoryViewModel>>(_key);

        private void SetCache(List<CategoryViewModel> toCache)
            => _cache.Set(_key, toCache);

        public bool IsCategory(Guid categoryId)
            => _dbContext.Categories.Any(x => x.Id == categoryId);

        public async Task Add(Guid id, string name)
        {
            var model = new Category()
            {
                Id = id,
                Name = name
            };

            await _dbContext.Categories.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            var cached = GetCache();

            if(cached != null)
            {
                cached.Add(new CategoryViewModel()
                {
                    Id = model.Id,
                    Name = model.Name
                });

                SetCache(cached);
            }
        }

        public async Task<List<CategoryViewModel>> Get()
        {
            var result = GetCache();

            if (result != null)
                return result;

            result = await _dbContext.Categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            SetCache(result);
            return result;
        }
    }
}
