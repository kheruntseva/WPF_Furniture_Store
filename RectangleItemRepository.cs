using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace mywpf
{
    public class RectangleItemRepository : Repository<RectangleItem>, IRectangleItemRepository
    {
        public RectangleItemRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<RectangleItem> GetWithTranslations(string languageCode)
        {
            return Context.Set<RectangleItem>()
                .Include(i => i.ItemTranslations)
                .ToList()
                .Select(item =>
                {
                    var translation = item.ItemTranslations
                        .FirstOrDefault(t => t.LanguageCode == languageCode);
                    return new RectangleItem(item)
                    {
                        Name = translation?.Name ?? item.Name,
                        Description = translation?.Description,
                        Category = translation?.Category,
                        Availability = translation?.Availability
                    };
                });
        }
    }
}