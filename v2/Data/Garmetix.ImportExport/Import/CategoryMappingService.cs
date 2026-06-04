using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.ImportExports.Services
{
    public class CategoryMappingService
    {
        private readonly DatabaseContext _context;

        // In-memory caches to prevent DB calls
        private ConcurrentDictionary<string, Guid> _categoryCache;
        private ConcurrentDictionary<string, Guid> _subCategoryCache;
        private Guid _companyId;

        public CategoryMappingService(DatabaseContext context)
        {
            _context = context;
        }

        // Call this ONCE before you start the import loop
        public async Task InitializeCacheAsync(Guid companyId)
        {
            _companyId = companyId;

            // Load all existing categories from DB into memory
            var categories = await _context.ProductCategories.ToListAsync();
            _categoryCache = new ConcurrentDictionary<string, Guid>(
                categories.ToDictionary(c => c.Name.Trim().ToLower(), c => c.Id));

            // Load all existing subcategories from DB into memory
            var subCategories = await _context.ProductSubCategories.ToListAsync();
            _subCategoryCache = new ConcurrentDictionary<string, Guid>(
                subCategories.ToDictionary(c => c.Name.Trim().ToLower(), c => c.Id));
        }

        /// <summary>
        /// Gets the Category ID. If it doesn't exist in DB, creates it.
        /// </summary>
        public async Task<Guid> GetOrCreateCategoryAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) categoryName = "Uncategorized";

            string key = categoryName.Trim().ToLower();

            // 1. Check Memory Cache
            if (_categoryCache.TryGetValue(key, out Guid id))
            {
                return id;
            }

            // 2. Not found. Create in DB, Save, and Add to Cache
            var newCategory = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = categoryName.Trim(),
                CompanyId = _companyId
            };

            _context.ProductCategories.Add(newCategory);
            await _context.SaveChangesAsync(); // Save immediately so foreign keys work

            _categoryCache.TryAdd(key, newCategory.Id);
            return newCategory.Id;
        }

        /// <summary>
        /// Gets the SubCategory ID. If it doesn't exist in DB, creates it.
        /// </summary>
        public async Task<Guid> GetOrCreateSubCategoryAsync(string subCategoryName)
        {
            if (string.IsNullOrWhiteSpace(subCategoryName)) return Guid.Empty;

            string key = subCategoryName.Trim().ToLower();

            if (_subCategoryCache.TryGetValue(key, out Guid id))
            {
                return id;
            }

            var newSub = new ProductSubCategory
            {
                Id = Guid.NewGuid(),
                Name = subCategoryName.Trim(),
                CompanyId = _companyId
            };

            _context.ProductSubCategories.Add(newSub);
            await _context.SaveChangesAsync();

            _subCategoryCache.TryAdd(key, newSub.Id);
            return newSub.Id;
        }
    }
}


